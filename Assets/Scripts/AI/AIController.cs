using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using Devdog.InventoryPro.UnityStandardAssets;
[RequireComponent(typeof(HealthManager))]
//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SkillSet))]
//[RequireComponent(typeof(ThirdPersonCharacter))]
[RequireComponent(typeof(AIActionDecider))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    public int Level;
    public float MinimumAttackTimeDifference = 2f;

    public List<Attack> attacks;
    private SkillSet _skillSet;
    private Animator _anim;
    private NavMeshAgent _agent;
    private ThirdPersonCharacter thirdPerson;
    
    public AIActionDecider Decider;

    public bool Moving;
    public bool Frozen;

    private void Start()
    {
        _skillSet = GetComponent<SkillSet>();
        _anim = GetComponent<Animator>();
        if (!_anim) _anim = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        attacks = new List<Attack>();

        foreach (Attack a in _skillSet.attacks)
        {
            attacks.Add(Instantiate(a));
        }

        Decider = GetComponent<AIActionDecider>();
        Moving = false;
        thirdPerson = GetComponent<ThirdPersonCharacter>();
        if (!thirdPerson) thirdPerson = GetComponentInChildren<ThirdPersonCharacter>();
    }

    void Update()
    {
        ChooseAction();
        MoveUpdate();
    }

    private void Life()
    {
        enabled = true;
    }

    private void MoveUpdate()
    {
        Vector3 vel = _agent.velocity * 0.5f;
        if (Decider.combat)
        {
            _agent.updateRotation = false;
            Decider.FaceTarget(AIActionDecider.Player.transform.position);
        }
        else
            _agent.updateRotation = true;
        if (!_skillSet.CheckAttack() || Frozen)
            thirdPerson.Move(vel, false, false);
    }
    

    private void ChooseAction()
    {
        Decider.Tick();         
    }

}