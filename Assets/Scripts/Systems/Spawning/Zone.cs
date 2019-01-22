using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zone is a rendering and spawning class. It is attached to a gameObject representing an area
/// All of its children are part of the Zone and render/disable based on the Zone's status
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class Zone : MonoBehaviour {
    public bool PlayerInZone;
    public bool ActiveZone;

    public Transform RespawnPoint;
    private BoxCollider _collider;

	// Use this for initialization
	void Start () {
        if (!RespawnPoint)
        {
            var rp = GetComponentInChildren<RespawnPoint>();
            if (rp) RespawnPoint = rp.transform;
        }
        _collider = GetComponent<BoxCollider>();
        _collider.enabled = true;
        _collider.isTrigger = true;
	}

    private void OnDisable()
    {
        ActiveZone = false;
    }

    private void OnEnable()
    {
        ActiveZone = true;
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
    

    /// <summary>
    /// The Trigger interacts only with the player layer. All others are ignored.
    /// The player has entered the Zone, deactivate non-neightboring zones and activate all neighbors
    /// </summary>
    /// <param name="other"></param>
    /// TODO: Change Deactivate so it is not occuring during physics update
    internal void OnTriggerEnter(Collider other)
    {
        PlayerInZone = true;
        GameManager.CurrentZone = this;
    }

    internal void OnTriggerExit(Collider other)
    {
        PlayerInZone = false;
    }
}
