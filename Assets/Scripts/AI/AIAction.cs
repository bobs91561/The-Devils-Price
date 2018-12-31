using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// A base class for all actions taken by an AI
/// </summary>
public abstract class AIAction : ScriptableObject
{
    public float priority;
    public bool isActive;

    public GameObject g;
    public AIActionDecider decider;

    public abstract bool ActionFeasible();
    public abstract bool Tick();
    public virtual void Initialize(GameObject obj = null)
    {
        g = obj;
        decider = g.GetComponent<AIActionDecider>();
        isActive = false;
    }

    public virtual void SetActive()
    {
        isActive = !isActive;
    }
}
