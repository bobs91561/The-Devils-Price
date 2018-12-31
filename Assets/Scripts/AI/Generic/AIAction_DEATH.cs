using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "AIAction/DEATH")]
public class AIAction_DEATH : AIAction
{
    public override bool ActionFeasible()
    {
        if (!g) return false;
        return !g.GetComponent<HealthManager>().isAlive;
    }

    public override bool Tick()
    {
        g.GetComponent<AIController>().enabled = false;
        return ActionFeasible();
    }
}
