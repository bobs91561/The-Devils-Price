﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : ScriptableObject {

    //The attacking character
    [HideInInspector] public GameObject attacker;

    //This object has any ParticleSystems, Colliders, or other special effects
    public GameObject objectToGenerate;
    public AudioClip audioToPlay;

    protected GameObject objectGenerated;
    protected GameObject objectGeneratedSecondary;

    public float damage;
    public float coolDown;
    public float soulStrengthRequired;
    public float demonicPowerRequired;

    public float maxForwardDistance;
    public float areaOfAttack;
    public float attackSpeed;

    public string animationKey;

    protected GameObject targetObject;

    protected SkillSet skillSet;

    /// <summary>
    /// Call for any attack
    /// </summary>
    public abstract void UseAttack();

    public virtual void TriggerAnimation()
    {
        Animator a = attacker.GetComponent<Animator>();
        if (!a) a = attacker.GetComponentInChildren<Animator>();
        if (a)
        {
            a.SetTrigger(animationKey);
        }
        if (audioToPlay)
        {
            var audio = a.GetComponent<AudioSource>();
            if (audio)
            {
                audio.clip = audioToPlay;
            }
        }
    }

    public bool CheckRequirements(float cD, float sSR)
    {
        return (cD >= coolDown && sSR >= soulStrengthRequired);
    }

    public void Interrupt()
    {
        if (objectGenerated) Destroy(objectGenerated);
        if (objectGeneratedSecondary) Destroy(objectGeneratedSecondary);
    }

    public void Target(GameObject g)
    {
        targetObject = g;
        var skill = targetObject.GetComponent<SkillSet>();
        if (objectGenerated)
        {
            Vector3 p;
            if(skill && skill.characterCenter)
            {
                p = skill.characterCenter.transform.position;
            }
            else
                p = g.transform.position;
            Vector3 t = attacker.GetComponent<SkillSet>().castingObject.transform.position;
            Vector3 forward = (g.transform.position - attacker.GetComponent<SkillSet>().castingObject.transform.position).normalized;
            //forward.y = 0f;
            objectGenerated.transform.forward = forward;
            
        }
    }

    public void Target(Vector3 point)
    {
        if (objectGenerated)
        {
            Vector3 forward = (point - attacker.GetComponent<SkillSet>().castingObject.transform.position).normalized;
            forward.y = 0f;
            objectGenerated.transform.forward = forward;

        }
    }

    public void FaceDirection()
    {
        
    }

    public virtual void End()
    {

    }

    public virtual void Initialize(GameObject g)
    {
        attacker = g;
        skillSet = g.GetComponent<SkillSet>();
    }
}
