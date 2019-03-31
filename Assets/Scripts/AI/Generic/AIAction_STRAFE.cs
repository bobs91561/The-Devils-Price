using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/STRAFE")]
public class AIAction_STRAFE : AIAction
{

    private int _direction;             //-1 for left, 1 for right
    private bool _directionChosen;
    private bool _directionSet;
    private Vector3 g_position;
    private Vector3 n_position;
    private NavMeshHit _hit;

    public float MinStrafeRange, MaxStrafeRange;
    public string StrafeAnimation;

    public override bool ActionFeasible()
    {
        //Feasible if in combat, within a distance range of player
        if (!IsPlayerPresent()) return false;
        return decider.combat && CheckRange() && !_skillSet.CheckAttack();
    }

    public override bool Tick()
    {
        if (!isActive)
        {
            ChooseDirection();
            SetAnimation();
            return true;
        }
        else if (!_directionSet)
        {
            SetDirection();
        }
        else
        {
            //Update the forward direction of the AI to look at player
            decider.FaceTarget(AIActionDecider.Player.transform.position);
        }
        return _directionChosen;
    }

    private void SetDirection()
    {
        _agent.velocity = new Vector3(_direction * 2f, 0, 0);
    }

    private void ChooseDirection()
    {
        //Randomly pick between left or right strife direction
        //Set the direction
        g_position = g.transform.position;
        _direction = Random.Range(-1, 1);
        if (_direction == 0) _direction++;
        n_position = g_position + new Vector3(_direction, 0, 0);
        bool _blocked = NavMesh.Raycast(g_position, n_position, out _hit, NavMesh.AllAreas);
        if (_blocked == true)
        {
            if (_direction == 1)
                _direction = -1;
            else
                _direction = 1;
            n_position = g_position + new Vector3(_direction, 0, 0);
            _blocked = NavMesh.Raycast(g_position, n_position, out _hit, NavMesh.AllAreas);
            if (_blocked == true)
                _direction = 0;
        }
        _directionChosen = true;
        _directionSet = false;
    }

    private bool CheckRange()
    {
        float d = Vector3.Distance(decider.AI.transform.position, AIActionDecider.Player.transform.position);
        return (d >= MinStrafeRange && d <= MaxStrafeRange);
    }

    private void SetAnimation()
    {
        _animator.SetBool(StrafeAnimation, _directionChosen);
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _direction = 0;
        _directionChosen = false;
    }

    public override void SetActive()
    {
        if (isActive)
        {
            _directionChosen = false;
            _directionSet = false;
        }
        else _agent.ResetPath();
        base.SetActive();
        decider.combatMoveActive = isActive;
        //if (!isActive)
          //  SetAnimation();
    }
}
