using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Transfer")]
public class Transfer : Buff
{
    private GameObject drainHitbox;

    public override void UseAttack()
    {
        base.UseAttack();
        Transform debuffTarget;
        if (SelfTargeting)
        {
            debuffTarget = attacker.transform;
        }
        else
            debuffTarget = targetObject.transform;

        //Instantiate effect
        var d = Instantiate(drainHitbox);
        objectGenerated = d;

        //Set transform values
        SetTransform();


        //Apply values to special hitbox
        d.GetComponentInChildren<DrainHitbox>().Initialize(debuffTarget.gameObject, effectGrounded, SingleMoment, ReverseOnComplete, damage, duration);
    }
}
