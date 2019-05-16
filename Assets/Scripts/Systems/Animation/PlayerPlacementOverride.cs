using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacementOverride : TimelineOverrides
{
    public bool MoveToThisLocation;
    public GameObject MoveHere;
    public GameObject Player;
    
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void OverrideTimeline()
    {
        if (MoveToThisLocation)
        {
            Player.transform.position = this.transform.position;
        }
        else
        {
            Player.transform.position = MoveHere.transform.position;
        }
    }
}
