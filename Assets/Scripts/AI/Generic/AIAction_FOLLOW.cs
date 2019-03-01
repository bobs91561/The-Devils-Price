using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/FOLLOW")]
public class AIAction_FOLLOW : AIAction 
{
    public float minDistanceBetween;

    private Vector3 _behindAIPosition;

    public override bool ActionFeasible()
    {
        return (!decider.combat);
    }

    public override bool Tick()
    {
        Vector3 agentPos = FindObjectOfType<NavMeshAgent>().transform.position;
        _behindAIPosition = agentPos + (minDistanceBetween * Vector3.Normalize(agentPos));
        return _agent.SetDestination(_behindAIPosition);
    }

}
