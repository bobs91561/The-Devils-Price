using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCharactersOverride : TimelineOverrides
{
    private bool mode = false;
    public List<GameObject> elements;

    public override void OverrideTimeline()
    {
        
        GameManager.Player.SetActive(mode);
        foreach(GameObject obj in elements){
            obj.SetActive(mode);
        }
        mode = !mode;
    }
}