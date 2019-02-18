using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : Cast
{
    public float duration;
    public bool SelfTargeting;

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

        //Apply values to special hitbox
    }
}
