using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// This behavior is for special objects like targets that are relevant to quests.
/// When the object dies, this script tells a specific dialoguesystemtrigger to enable
/// </summary>
public class UpdateQuestOnDeath : OnDeathBase {

    public DialogueSystemTrigger triggerToEnable;

    public override void OnDeath()
    {
        
        if (!triggerToEnable) return;
        triggerToEnable.enabled = true;
    }


    public void Die()
    {
        base.OnDeath();
    }
}
