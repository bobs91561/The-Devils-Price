using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Buff")]
public class Buff : Cast
{

    [Tooltip("How long the Buff lasts")] public float duration;
    [Tooltip("Whether the Buff is applied all at once one time.")]public bool SingleMoment;
    [Tooltip("If the Buff is targeting the caster or another gameobject.")] public bool SelfTargeting;
    [Tooltip("Should the effects of the buff be reversed after the duration")] public bool ReverseOnComplete;


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


        //Apply values to special hitbox
        g.GetComponentInChildren<BuffHitbox>().Initialize(target.gameObject, effectGrounded, SingleMoment, ReverseOnComplete, damage, duration);
    }
}
