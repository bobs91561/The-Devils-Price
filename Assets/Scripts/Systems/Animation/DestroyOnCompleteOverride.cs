using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class DestroyOnCompleteOverride : TimelineOverrides
{
    public GameObject[] Objects;

    public override void OverrideTimeline()
    {
        foreach (GameObject obj in Objects) {
            Destroy(obj);
        }
    }
}
