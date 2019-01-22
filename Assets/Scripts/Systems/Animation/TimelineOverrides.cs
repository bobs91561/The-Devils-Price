using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Makes calls to the GameManager class such as Fading to the cutscene, resetting the player, etc.
/// </summary>
public abstract class TimelineOverrides : MonoBehaviour
{
    public PlayableAsset Timeline;



    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        OverrideTimeline();
    }

    public abstract void OverrideTimeline();
}
