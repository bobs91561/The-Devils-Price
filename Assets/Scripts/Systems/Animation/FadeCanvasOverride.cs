using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvasOverride : TimelineOverrides
{
    private int CallNumber = 0;

    public override void OverrideTimeline()
    {
        GameManager.instance.FadeInScene();
    }
}
