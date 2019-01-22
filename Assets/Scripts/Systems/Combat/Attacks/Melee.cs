using System;
using System.Collections.Generic;
using UnityEngine;
using Systems.Combat;


[CreateAssetMenu (menuName ="Attacks/Melee")]
public class Melee : Attack
{
    public bool OneHit = true;
    public bool SwapSecondary = false;
    public bool DontEndAfterDestroy = false;
    public bool MultipleGenerations;
    public float HitboxDestroyTime;

    private GameObject meleeObject, meleeObjectSecondary;
    private SkillSet _skillSet;

    public override void UseAttack()
    {
        GameObject hb = Instantiate(objectToGenerate);
        objectGenerated = hb;
        hb.transform.SetParent(meleeObject.transform);
        hb.transform.localPosition = new Vector3(0,0,0);
        hb.GetComponent<Hitbox>().Initialize(attacker, damage + _skillSet.WeaponDamage1, OneHit,HitboxDestroyTime, DontEndAfterDestroy);

        if(MultipleGenerations && meleeObjectSecondary)
        {
            GameObject hb2 = Instantiate(objectToGenerate);
            objectGeneratedSecondary = hb2;
            hb2.transform.SetParent(meleeObjectSecondary.transform);
            hb2.transform.localPosition = new Vector3(0, 0, 0);
            hb2.GetComponent<Hitbox>().Initialize(attacker, damage + _skillSet.WeaponDamage2, OneHit, HitboxDestroyTime, DontEndAfterDestroy);
        }
    }

    public override void End()
    {
        base.End();
        Interrupt();
    }

    public override void Initialize(GameObject g)
    {
        base.Initialize(g);
        _skillSet = attacker.GetComponent<SkillSet>();
        objectToGenerate = _skillSet.HitboxToGenerate;

        if (!SwapSecondary || !_skillSet.meleeObjectSecondary)
        {
            meleeObject = _skillSet.meleeObject;
            meleeObjectSecondary = _skillSet.meleeObjectSecondary;
        }
        else
        {
            meleeObject = _skillSet.meleeObjectSecondary;
            meleeObjectSecondary = _skillSet.meleeObject;
        }
    }
}
