using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/SEARCH")]
public class AIAction_SEARCH : AIAction
{
    private float m_TimeSpentSearching;
    private float m_DistanceSearched;
    public float MaxSearchTime;
    public float MaxSearchRadius;
    public float MinSearchRadius;

    private HealthManager m_PlayerHealth;

    private NavMeshPath m_Path;

    private Vector3 m_Goal;

    private bool m_RedetermineGoal = false;

    public override bool ActionFeasible()
    {
        return !decider.combat && decider.RecentCombat && m_PlayerHealth.isAlive;
    }

    public override bool Tick()
    {
        if (_agent.hasPath)
        {
            UpdateTime();
            if (_agent.remainingDistance <= _agent.stoppingDistance || WithinMinDistance())
            {
                // Reached its search location, redetermine search location
                return StopPath();
            }

            _agent.isStopped = CheckMovementTime();
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
            return _agent.SetPath(m_Path);
        }

    }

    private void UpdateTime()
    {
        m_TimeSpentSearching += Time.deltaTime * 0.1f;
    }

    private void CheckRadius()
    {

    }

    /// <summary>
    /// Returns true if the AI is within the minimum distance allowed to the player. False otherwise
    /// </summary>
    /// <returns></returns>
    private bool WithinMinDistance()
    {
        return (Vector3.Distance(g.transform.position, m_Goal) <= MinSearchRadius);
    }

    public bool ChooseNewPath()
    {
        if (_agent.hasPath) StopPath();
        NavMeshPath p = new NavMeshPath();
        m_Path = p;
        var b = _agent.CalculatePath(ChoosePathDirection(), m_Path);
        return b;
    }

    private bool StopPath()
    {
        _agent.velocity = new Vector3();
        _agent.ResetPath();
        return false;
    }

    private bool CheckMovementTime()
    {
        return m_TimeSpentSearching >= MaxSearchTime;
    }

    private Vector3 ChoosePathDirection()
    {
        Vector3 lkp = decider.LastKnownPosition;
        return lkp;
    }

    public override void SetActive()
    {
        if (isActive)
        {
            decider.RecentCombat = false;
            StopPath();
            m_RedetermineGoal = false;
        }
        base.SetActive();
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        m_PlayerHealth = AIActionDecider.Player.GetComponent<HealthManager>();
        m_Path = new NavMeshPath();
    }
}
