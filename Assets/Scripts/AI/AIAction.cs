using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A base class for all actions taken by an AI
/// </summary>
public abstract class AIAction : ScriptableObject
{
    public float priority;
    public bool isActive;

    public GameObject g;
    public AIActionDecider decider;

    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected SkillSet _skillSet;


    public abstract bool ActionFeasible();
    public abstract bool Tick();
    public virtual void Initialize(GameObject obj = null)
    {
        g = obj;
        isActive = false;
        decider = g.GetComponent<AIActionDecider>();

        _agent = g.GetComponent<NavMeshAgent>();
        _skillSet = decider.skillSet;
        _animator = g.GetComponent<Animator>();
        if (!_animator) _animator = g.GetComponentInChildren<Animator>();
    }

    public virtual void SetActive()
    {
        isActive = !isActive;
    }

    public bool DependsOnPlayer = false;

    protected bool IsPlayerPresent()
    {
        if (AIActionDecider.Player) return true;
        return false;
    }
}
