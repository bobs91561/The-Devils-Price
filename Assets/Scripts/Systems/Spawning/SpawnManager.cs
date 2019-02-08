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
    public float MinimumDistanceFromPlayer = 50f;
    public Queue<RespawnsOnDeath> _mRespawnRequests;
    public List<RespawnsOnDeath> _mRespawns;

	// Use this for initialization
	void Start () {
        _mRespawnRequests = new Queue<RespawnsOnDeath>();
        _mRespawns = new List<RespawnsOnDeath>();
        EventManager.FellOffWorld += ResetPlayer;
        EventManager.DeathAction += RespawnPlayer;
        instance = this;
        StartCoroutine(ConsiderRequests());
	}

    IEnumerator ConsiderRequests()
    {
        yield return new WaitWhile(() => _mRespawnRequests.Count <= 0);
        Debug.Log("Considering requests");
        AcceptRequests();
        Respawn();
        StartCoroutine(ConsiderRequests());
    }

    private void AcceptRequests()
    {
        RespawnsOnDeath request;
        Queue<RespawnsOnDeath> delayed = new Queue<RespawnsOnDeath>();
        while(_mRespawnRequests.Count > 0)
        {
            request = _mRespawnRequests.Dequeue();
            if (!request) continue;
            //Respawn conditions
            //Zone inactive? Continue, request is handled by enable
            //Zone is active, player is nearby? Delay request.
            //Zone is active, player is not nearby? Accept request.
            if (!request.zone || !request.zone.ActiveZone) continue;
            if (request.zone.ActiveZone)
            {
                var accept = CheckPlayerDistance(request);
                Debug.Log(accept);
                if (accept) _mRespawns.Add(request);
                else delayed.Enqueue(request);
            }
        }
        _mRespawnRequests = delayed;
    }

    private bool CheckPlayerDistance(RespawnsOnDeath request)
    {
        //if (!request.zone.PlayerInZone) return true;
        var dist = Vector3.Distance(request.gameObject.transform.position, GameManager.Player.transform.position);
        return dist >= MinimumDistanceFromPlayer * request.zone.Data.SpawnRate;
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
        Debug.Log("Request received");
    }

    private void ResetPlayer()
    {
        if (GameManager.CurrentZone.RespawnPoint)
        {
            Transform t = GameManager.CurrentZone.RespawnPoint;
            GameManager.Player.transform.position = t.position;
        }

    }

    private void RespawnPlayer()
    {
        var zone = GameManager.CurrentZone;
        if (zone && zone.RespawnPoint)
        {
            //Use the respawn point in the current zone
            StartCoroutine(RespawnAtRespawnPoint(zone.RespawnPoint));
        }
        else
        {
            StartCoroutine(RespawnAtRespawnPoint());
        }
    }

    IEnumerator RespawnAtRespawnPoint(Transform rp)
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.FadeInScene();
        yield return new WaitForSeconds(1f);
        GameManager.Player.transform.position = rp.position;
        rp.gameObject.GetComponent<RespawnPoint>().Respawn();
    }

    IEnumerator RespawnAtRespawnPoint()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.FadeInScene();
        yield return new WaitForSeconds(1f);
        //Find the closest respawn point
        var rps = GameObject.FindGameObjectsWithTag("RespawnPoint");
        RespawnPoint rp = null;
        Vector3 pv = GameManager.Player.transform.position;
        float min = Mathf.Infinity;
        foreach (GameObject respawns in rps)
        {
            var v = Vector3.Distance(respawns.transform.position, pv);
            if (v < min)
            {
                rp = respawns.GetComponent<RespawnPoint>();
            }
        }
        if (rp) rp.Respawn();
    }

}
