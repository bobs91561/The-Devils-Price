using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zone is a rendering and spawning class. It is attached to a gameObject representing an area
/// All of its children are part of the Zone and render/disable based on the Zone's status
/// </summary>
public class Zone : MonoBehaviour {

    public List<Zone> NeighborZones;
    public bool PlayerInZone;
    public bool ActiveZone;

    public Transform RespawnPoint;

	// Use this for initialization
	void Start () {
		
	}
    /// <summary>
    /// Deactivates this Zone
    /// </summary>
    public void Deactivate()
    {
        if (gameObject)
        {
            gameObject.SetActive(false);
            ActiveZone = false;
        }
    }

    public void CheckDuplicates()
    {
        if (NeighborZones.Contains(this)) NeighborZones.Remove(this);
    }

    /// <summary>
    /// The Trigger interacts only with the player layer. All others are ignored.
    /// The player has entered the Zone, deactivate non-neightboring zones and activate all neighbors
    /// </summary>
    /// <param name="other"></param>
    internal void OnTriggerEnter(Collider other)
    {
        PlayerInZone = true;
        GameManager.CurrentZone = this;
        foreach (Zone n in NeighborZones)
        {
            n.ActiveZone = true;
            n.gameObject.SetActive(true);
        }
        List<Zone> actives = GameManager.ActiveZones;
        foreach(Zone z in actives)
        {
            if (z!=this && !NeighborZones.Contains(z)) z.Deactivate();
        }
        GameManager.ActiveZones = NeighborZones;
        GameManager.ActiveZones.Add(this);
        CheckDuplicates();
    }

    internal void OnTriggerExit(Collider other)
    {
        PlayerInZone = false;
        if (GameManager.CurrentZone == this)
        {
            EventManager.OffWorld();
        }
    }
}
