using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/STRAFE")]
public class AIAction_STRAFE : AIAction {

    private NavMeshAgent _agent;
    private Animator _anim;
    private SkillSet _skillSet;
    private int _direction;             //-1 for left, 1 for right
    private bool _directionChosen;

    public float MinStrafeRange, MaxStrafeRange;
    public string StrafeAnimation;

    public override bool ActionFeasible()
    {
        //Feasible if in combat, within a distance range of player
        return decider.combat && CheckRange() && !_skillSet.CheckAttack();
    }

    public override bool Tick()
    {
        if (!isActive)
        {
            ChooseDirection();
            SetAnimation();
            SetDirection();
            return true;
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
        _agent.velocity = new Vector3(_direction, 0, 0);
    }

    private void ChooseDirection()
    {
        //Randomly pick between left or right strife direction
        //Set the direction
        _direction = Random.Range(-1,1);
        _directionChosen = true;
    }

    private bool CheckRange()
    {
        float d = Vector3.Distance(decider.AI.transform.position, AIActionDecider.Player.transform.position);
        return (d >= MinStrafeRange && d <= MaxStrafeRange);
    }

    private void SetAnimation()
    {
        _anim.SetBool(StrafeAnimation, _directionChosen);
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _agent = decider.AI.GetComponent<NavMeshAgent>();
        _anim = decider.AI.GetComponent<Animator>();
        _skillSet = decider.AI.GetComponent<SkillSet>();
        _direction = 0;
        _directionChosen = false;
    }

    public override void SetActive()
    {
        if (isActive) _directionChosen = false;
        else _agent.ResetPath();
        base.SetActive();
        decider.combatMoveActive = isActive;
        if (!isActive)
            SetAnimation();
    }
}
