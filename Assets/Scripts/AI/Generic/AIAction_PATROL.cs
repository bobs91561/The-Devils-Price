using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName ="AIAction/PATROL")]
public class AIAction_PATROL : AIAction {
    private NavMeshAgent _agent;

    public float TiredThresholdToStop;
    public float TiredThresholdToStart;

    private NavMeshPath path;

    public override bool ActionFeasible()
    {
        //feasible conditions:
        //not in combat
        //tiredness is below threshold to start
        if (decider.combat && _agent.hasPath && !decider.combatMoveActive) StopPath();
        return (!decider.combat && decider.tiredness <= TiredThresholdToStart);
    }

    public override bool Tick()
    {
        if(_agent.hasPath)
        {
            decider.tiredness += Time.deltaTime * .1f;
            if (_agent.remainingDistance <= _agent.stoppingDistance)
                return StopPath();
            
            _agent.isStopped = decider.tiredness >= TiredThresholdToStop;

            return true;
        }
        else if(!isActive)
        {
            //The agent has no path and was not active prior to this tick
            return ChooseNewPath();
        }
        else
        {
            //The agent has no path and was active prior to this tick
            //There exists a calculated path stored in variable
            //Set the agent's path
            //if (path == null) ChooseNewPath();
            return _agent.SetPath(path);
        }
    }

    public bool ChooseNewPath()
    {
        if (_agent.hasPath) StopPath();
        NavMeshPath p = new NavMeshPath();
        path = p;
        return _agent.CalculatePath(decider.FindNearbyPatrolPoint(), path);
    }

    private bool StopPath()
    {
        _agent.velocity = new Vector3();
        _agent.ResetPath();
        decider.tiredness = TiredThresholdToStop;
        return false;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _agent = g.GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }
}
