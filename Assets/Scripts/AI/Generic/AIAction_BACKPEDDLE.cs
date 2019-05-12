﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(menuName = "AIAction/BACKPEDDLE")]
public class AIAction_BACKPEDDLE : AIAction
{

    private AIAttackController _attackController;
    private NavMeshPath _path;

    public float minDistanceFromPlayer;
    public float MaxMovementTime;


    public override bool ActionFeasible()
    {
        //check if the AI is iwithin the min distance of the player, return false if so
        //Check if the AI isActive but the AI is now attacking
        //Return true only if in combat and the AI not attacking
        if (!IsPlayerPresent() || !decider.combat) return false;
        if (WithinMinDistance()) return false;
        //return decider.combat && !_skillSet.CheckAttack();
        return !_skillSet.CheckAttack();
    }

    public override bool Tick()
    {
        if (_agent.hasPath)
        {
            decider.combatApproach += Time.deltaTime * .1f;
            if (_agent.remainingDistance <= _agent.stoppingDistance || WithinMinDistance())
                return StopPath();

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
            return _agent.SetPath(_path);
        }
    }
    /// <summary>
    /// Returns true if the AI is within the minimum distance allowed to the player. False otherwise
    /// </summary>
    /// <returns></returns>
    private bool WithinMinDistance()
    {
        return (Vector3.Distance(g.transform.position, AIActionDecider.Player.transform.position) >= minDistanceFromPlayer);
    }
    public bool ChooseNewPath()
    {
        if (_agent.hasPath) StopPath();
        NavMeshPath p = new NavMeshPath();
        _path = p;
        var b = _agent.CalculatePath(ChoosePathDirection(), _path);
        return b;
    }

    private bool StopPath()
    {
        _agent.velocity = new Vector3();
        _agent.ResetPath();
        decider.combatApproach = MaxMovementTime;
        return false;
    }

    private bool CheckMovementTime()
    {
        return decider.combatApproach >= MaxMovementTime;
    }

    private Vector3 ChoosePathDirection()
    {
        Vector3 playerPos = AIActionDecider.Player.transform.position;
        //playerPos.y = 0;
        playerPos.x *= -1;
        playerPos.z *= -1;
        return playerPos;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _attackController = g.GetComponent<AIAttackController>();
        _path = new NavMeshPath();
    }

    public override void SetActive()
    {
        if (isActive) StopPath();
        base.SetActive();
        decider.combatMoveActive = isActive;
        decider.combatApproach = 0f;

    }
}

