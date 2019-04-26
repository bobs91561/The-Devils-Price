using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : Cast
{
    public float duration;
    public bool SingleMoment;
    public bool SelfTargeting;
    public bool ReverseOnComplete;

    public override void UseAttack()
    {
        //Get transform of object to instantiate the effect over
        Transform target;
        if (SelfTargeting)
        {
            target = attacker.transform;
        }
        else
            target = targetObject.transform;

        //Instantiate effect
        var g = Instantiate(objectToGenerate);
        objectGenerated = g;

        //Set transform values
        SetTransform();

        //Change layer to collide with the caster

        //Apply values to special hitbox
        g.GetComponentInChildren<BuffHitbox>().Initialize(target.gameObject, effectGrounded, SingleMoment, ReverseOnComplete, damage, duration);
    }
}
