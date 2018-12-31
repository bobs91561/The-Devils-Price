using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyConditional : MonoBehaviour {

    private AIActionDecider _mDecider;
    public bool TriggeredByConversation;

	// Use this for initialization
	void Start () {
        _mDecider = GetComponent<AIActionDecider>();
        _mDecider.Friendly = true;
        EventManager.Aggression += OnAggression;
	}

    /// <summary>
    /// Called by dialogue system, other scripts when player attacks or triggers confrontation
    /// </summary>
    public void OnAggression()
    {
        _mDecider.Friendly = false;
    }
}
