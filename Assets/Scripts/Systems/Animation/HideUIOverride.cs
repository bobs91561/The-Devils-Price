using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class HideUIOverride : TimelineOverrides
{
    public override void OverrideTimeline()
    {
        //Get the UIs from the GameManager and hide them
        var dm = GameManager.GetDialogueManager();
        var nav = GameManager.GetNavigator();
        if (nav)
        {
            nav.GetComponentInChildren<Canvas>().enabled = false;
        }
        if (dm)
        {
            var questlog = dm.GetComponentInChildren<StandardUIQuestTracker>();
            questlog.HideTracker();
            var crosshair = dm.GetComponentInChildren<CrossHair>();
            crosshair.gameObject.GetComponent<Image>().enabled = false;
            var healthbar = GameManager.Player.GetComponent<HealthManager>().healthBar;
            healthbar.Deactivate();
        }
    }
    
}
