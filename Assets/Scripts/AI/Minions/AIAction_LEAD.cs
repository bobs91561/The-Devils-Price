using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAction_LEAD : AIAction
{
    public Transform Destination;
    public Transform LeadObject;

    public float MaxDistanceToLead;
    public bool LeadAllowed = false;

    public override bool ActionFeasible()
    {
        return LeadAllowed;
    }

    public override bool Tick()
    {
        // Move logic
        // If the player is out of range, stay stationary
        return false;
    }

    public void Lead(Transform t)
    {
        LeadObject = t;
        LeadAllowed = true;
    }
    
}
