using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AIAction/ChangeMode")]
public class AIAction_CHANGEMODE : AIAction
{
    public float Threshold;
    public GameObject VisualEffect;

    public override bool ActionFeasible()
    {
        return false;
    }

    public override bool Tick()
    {
        throw new System.NotImplementedException();
    }
}
