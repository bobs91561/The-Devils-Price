using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class ChangeConversationSettingsOverride : TimelineOverrides
{
    DisplaySettings.SubtitleSettings.ContinueButtonMode currentMode;
    public override void OverrideTimeline()
    {
        GameObject dm = GameManager.GetDialogueManager();
        if(!dm)
        {
            Debug.Log("Dm was null ");
            return;
        }
        var controller = dm.GetComponent<DialogueSystemController>();
        currentMode = controller.displaySettings.conversationOverrideSettings.continueButton;
        if (currentMode == DisplaySettings.SubtitleSettings.ContinueButtonMode.Always)
            currentMode = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
        else
            currentMode = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        controller.displaySettings.conversationOverrideSettings.continueButton = currentMode;
    }
}
