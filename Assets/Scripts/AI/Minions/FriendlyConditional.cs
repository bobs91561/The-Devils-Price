using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyConditional : MonoBehaviour {

    private AIActionDecider _mDecider;
    public bool TriggeredByConversation;

    public DialogueSystemTrigger TriggerToEnable;

	// Use this for initialization
	void Start () {
        _mDecider = GetComponent<AIActionDecider>();
        _mDecider.Friendly = true;
	}

    /// <summary>
    /// Called by dialogue system, other scripts when player attacks or triggers confrontation
    /// </summary>
    public void OnAggression(bool fromAttack = false)
    {
        _mDecider.Friendly = false;
        //If triggered before conversation, set trigger
        if (TriggerToEnable && fromAttack) TriggerToEnable.enabled = true;
    }
}
