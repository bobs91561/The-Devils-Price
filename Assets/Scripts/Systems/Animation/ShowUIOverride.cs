using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class ShowUIOverride : TimelineOverrides
{
    private StandardUIQuestTracker questlog;
    private CrossHair crosshair;
    private HealthBar healthbar;
    private GameObject dm;

    private void Start()
    {
        dm = GameManager.GetDialogueManager();
        questlog = dm.GetComponentInChildren<StandardUIQuestTracker>();
        crosshair = dm.GetComponentInChildren<CrossHair>();
        healthbar = GameManager.Player.GetComponent<HealthManager>().healthBar;
    }
    public override void OverrideTimeline()
    {
        Debug.Log("Called Override");
        //Get the UIs from the GameManager and hide them
        var nav = GameManager.GetNavigator();
        if (nav)
        {
            nav.GetComponentInChildren<Canvas>().enabled = true;
        }
        if (dm)
        {
            GameManager.instance.VerifyDialogueDisplay();
            crosshair.gameObject.GetComponent<Image>().enabled = true;
            healthbar.Activate();
        }
    }
}
