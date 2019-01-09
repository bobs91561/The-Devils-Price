using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/RETURN")]
public class AIAction_RETURN : AIAction
{
    private NavMeshAgent _agent;
    private NavMeshPath path;
    private Vector3 _position;
    private Transform _transform;

    public override bool ActionFeasible()
    {
        return !decider.combat && CheckRange() && decider.RecentCombat;
    }
    public override bool Tick()
    {
        if (_agent.hasPath)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
                return StopPath();
            return true;
        }
        else if (!isActive)
        {
            //The agent has no path and was not active prior to this tick
            return ChooseNewPath();
        }
        else
        {
            //The agent has no path and was active prior to this tick
            //There exists a calculated path stored in variable
            //Set the agent's path
            return _agent.SetPath(path);
        }
    }

    public bool ChooseNewPath()
    {
        if (_agent.hasPath) StopPath();
        NavMeshPath p = new NavMeshPath();
        path = p;
        return _agent.CalculatePath(_position, path);
    }

    private bool StopPath()
    {
        _agent.velocity = new Vector3();
        _agent.ResetPath();
        return false;
    }

    private bool CheckRange()
    {
        return Vector3.Distance(_position, _transform.position) >= 5f;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _agent = g.GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        _transform = g.transform;
        _position = _transform.position;
    }

    public override void SetActive()
    {
        if (isActive) decider.RecentCombat = false;
        base.SetActive();
    }
}
