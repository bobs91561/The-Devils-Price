using System.Collections;
using System.Collections.Generic;
using Systems.Combat;
using UnityEngine;

[CreateAssetMenu(menuName ="Attacks/Cast")]
public class Cast : Attack
{
    public bool effectGrounded;
    private CastingObject cast;
    private GameObject castingObject;
    private float damageMultiplier;

    public override void UseAttack()
    {
        //Generate effect at casting point location
        GameObject g = Instantiate(objectToGenerate);
        objectGenerated = g;

        damageMultiplier = skillSet.CastingDamage;

        Vector3 castingPoint = castingObject.transform.position;

        Vector3 attackerFwd = attacker.transform.forward;
        Vector3 attackerCtr = attacker.transform.position + attackerFwd * maxForwardDistance * .75f;
        attackerCtr.y = castingPoint.y;
        Vector3 rotated = (attackerCtr - castingPoint).normalized;
        g.transform.position = !effectGrounded ? castingPoint : attacker.transform.position;
        g.transform.forward = rotated;
        g.layer = attacker.layer;


        if (g.GetComponentInChildren<DamageForPreSetupObjects>()) UsePreSetup();
        else UseCustom();
    }

    private void UsePreSetup()
    {
        GameObject g = objectGenerated;
        g.GetComponentInChildren<DamageForPreSetupObjects>().gameObject.layer = attacker.layer;
        g.GetComponentInChildren<DamageForPreSetupObjects>().Initialize(damage * damageMultiplier, attacker);
        g.GetComponentInChildren<RFX1_TransformMotion>().CollidesWith = ~LayerMask.GetMask(LayerMask.LayerToName(attacker.layer), "Ignore Raycast");
        if (g.GetComponent<RFX1_Target>()) g.GetComponent<RFX1_Target>().Target = targetObject;
    }

    private void UseCustom()
    {
        GameObject g = objectGenerated;
        g.GetComponent<DamageForCustomObjects>().Initialize(damage * damageMultiplier, ~LayerMask.GetMask(LayerMask.LayerToName(attacker.layer), "Ignore Raycast"), attacker);
    }

    public override void Initialize(GameObject g)
    {
        base.Initialize(g);
        castingObject = skillSet.castingObject;
        cast = castingObject.GetComponent<CastingObject>();
    }
}
