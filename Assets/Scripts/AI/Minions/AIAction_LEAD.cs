using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/Special/LEAD")]
public class AIAction_LEAD : AIAction
{
    public Transform Destination;
    public Transform LeadObject;

    public float MaxDistanceToLead;
    public bool LeadAllowed = false;

    private NavMeshPath path;

    public override bool ActionFeasible()
    {
        return LeadAllowed;
    }

    public override bool Tick()
    {
        // Move logic if(_agent.hasPath)
        if (_agent.hasPath)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
                return StopPath();

            // If the player is out of range, stay stationary
            _agent.isStopped = !LeadObjectWithinRange();

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
            //if (path == null) ChooseNewPath();
            return _agent.SetPath(path);
        }
    }

    public bool ChooseNewPath()
    {
        if (_agent.hasPath) StopPath();
        NavMeshPath p = new NavMeshPath();
        path = p;
        return _agent.CalculatePath(Destination.position, path);
    }

    private bool StopPath()
    {
        _agent.velocity = new Vector3();
        _agent.ResetPath();
        return false;
    }

    private bool LeadObjectWithinRange()
    {
        return Vector3.Distance(LeadObject.position, g.transform.position) <= MaxDistanceToLead;
    }

    public void Lead(Transform t)
    {
        LeadObject = t;
        LeadAllowed = true;
    }


    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        path = new NavMeshPath();
    }

    public override void SetActive()
    {
        if (isActive) StopPath();
        base.SetActive();

    }

}
