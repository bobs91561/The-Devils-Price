using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Handles all respawn requests by observing what Zone the request originates from, what Zone the player is in,
///     how long the request has been waiting (queue and timestamp)
/// SpawnManager is a low-priority class and runs in the background
/// Coroutines decide which requests to approve
/// Update acts on approved requests
/// </summary>
public class SpawnManager : MonoBehaviour {
    public static SpawnManager instance;
    public float MinimumTimeToConsiderRespawn;
    public Queue<RespawnsOnDeath> _mRespawnRequests;
    public List<RespawnsOnDeath> _mRespawns;
    

	// Use this for initialization
	void Start () {
        _mRespawnRequests = new Queue<RespawnsOnDeath>();
        _mRespawns = new List<RespawnsOnDeath>();
        EventManager.FellOffWorld += ResetPlayer;
        instance = this;
	}

    private void LateUpdate()
    {
        if(_mRespawnRequests.Count > 0)
            AcceptRequests();
        if(_mRespawns.Count > 0)
            Respawn();
    }

    private void AcceptRequests()
    {
        RespawnsOnDeath request;
        bool delayFlag = false;
        int MaxChecks = _mRespawnRequests.Count;
        int ChecksDone = 0;
        while(_mRespawnRequests.Count > 0 && ChecksDone < MaxChecks) 
        {
            request = _mRespawnRequests.Dequeue();
            if (!request) continue;
            ChecksDone++;
            //is the zone active?
            //if not, remove the request
            if (!request.zone || !request.zone.ActiveZone)
                continue;
            //is the player in the zone?
            //if yes, consider delaying
            //if not, accept
            if (request.zone.PlayerInZone)
            {
                delayFlag = DelayRequest(request);
            }
            if (delayFlag)
            {
                _mRespawnRequests.Enqueue(request);
            }
            else
                _mRespawns.Add(request);
        }

    }

    private bool DelayRequest(RespawnsOnDeath request)
    {
        //what is the timestamp of the request?
        //how does it compare to the current time
        //if lengthy, accept
        //if not, delay
        return Time.time - request.TimeStamp >= MinimumTimeToConsiderRespawn;

    }

    public void RemoveRequest(RespawnsOnDeath r)
    {
        
    }

    /// <summary>
    /// Respawn processes the valid respawn requests
    /// </summary>
    private void Respawn()
    {
        foreach (RespawnsOnDeath r in _mRespawns)
            r.Respawn();
        _mRespawns.Clear();
    }
    /// <summary>
    /// Create a new respawn request
    /// </summary>
    /// <param name="respawn"></param>
    public void NewRequest(RespawnsOnDeath respawn)
    {
        _mRespawnRequests.Enqueue(respawn);
    }

    private void ResetPlayer()
    {
        Transform t = GameManager.CurrentZone.RespawnPoint;
        GameManager.Player.transform.position = t.position;

    }

}
