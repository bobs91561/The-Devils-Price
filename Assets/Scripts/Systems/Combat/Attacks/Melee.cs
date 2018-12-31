using System;
using System.Collections.Generic;
using UnityEngine;
using Systems.Combat;


[CreateAssetMenu (menuName ="Attacks/Melee")]
class Melee : Attack
{
    public bool OneHit = true;
    public bool MultipleGenerations;
    public float HitboxDestroyTime;
    public override void UseAttack()
    {
        GameObject meleeObject  = attacker.GetComponent<SkillSet>().meleeObject;
        GameObject meleeObjectSecondary = attacker.GetComponent<SkillSet>().meleeObjectSecondary;
        GameObject hb = Instantiate(objectToGenerate);
        objectGenerated = hb;
        hb.transform.SetParent(meleeObject.transform);
        hb.transform.localPosition = new Vector3(0,0,0);
        hb.GetComponent<Hitbox>().Initialize(attacker,damage, OneHit,HitboxDestroyTime);

        if(MultipleGenerations && meleeObjectSecondary)
        {
            GameObject hb2 = Instantiate(objectToGenerate);
            objectGeneratedSecondary = hb2;
            hb2.transform.SetParent(meleeObjectSecondary.transform);
            hb2.transform.localPosition = new Vector3(0, 0, 0);
            hb2.GetComponent<Hitbox>().Initialize(attacker, damage, OneHit, HitboxDestroyTime);
        }
    }

    public override void End()
    {
        base.End();
        Interrupt();
    }
}
