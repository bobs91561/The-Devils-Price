using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/FOLLOW")]
public class AIAction_FOLLOW : AIAction 
{
    public float minDistanceBetween;
    public GameObject target;

    private Vector3 targetPos;
    private NavMeshPath _path;

    public override bool ActionFeasible()
    {
        Debug.Log(target);
        if (!decider.combat || target == null) return false;
        return (WithinMinDistance());
    }

    public override bool Tick()
    {
        if (_agent.hasPath) 
        {
            return true;
        }
        else if (!isActive)
        {
            return ChooseNewPath();
        }
        else 
        {
            return _agent.SetPath(_path);
        }
    }

    private bool WithinMinDistance () 
    {
        return (Vector3.Distance(g.transform.position, target.transform.position) >= minDistanceBetween);
    }

    private bool ChooseNewPath ()
    {
        if (_agent.hasPath) StopPath();
        NavMeshPath p = new NavMeshPath();
        _path = p;
        var b = _agent.CalculatePath(ChoosePathDirection(), _path);
        return b;
    }

    private Vector3 ChoosePathDirection()
    {
        targetPos = target.transform.position;
        targetPos = targetPos + (minDistanceBetween * Vector3.Normalize(targetPos));
        return targetPos;
    }

    private bool StopPath()
    {
        _agent.velocity = new Vector3();
        _agent.ResetPath();
        return false;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _path = new NavMeshPath();
        target = decider.followTarget;
        Debug.Log(target);
    }

    public override void SetActive()
    {
        if (isActive) StopPath();
        base.SetActive();
    }
}
