using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Attacks/CastingAttackPreSetupCollision")]
public class CastingAttackPreSetupCollision : Attack {
    private CastingObject cast;
    private GameObject castingObject;
    private float damageMultiplier;

    public override void UseAttack()
    {
        GameObject g = Instantiate(objectToGenerate);
        objectGenerated = g;

        Vector3 castingPoint = skillSet.castingObject.transform.position;
        damageMultiplier = skillSet.CastingDamage;
        Vector3 attackerFwd = attacker.transform.forward;
        Vector3 attackerCtr = attacker.transform.position + attackerFwd*maxForwardDistance*.75f;
        attackerCtr.y = castingPoint.y;
        Vector3 rotated = (attackerCtr - castingPoint).normalized;
        g.transform.position = castingPoint;
        g.transform.forward = rotated;
        g.layer = attacker.layer;
        g.GetComponentInChildren<DamageForPreSetupObjects>().gameObject.layer = attacker.layer;
        g.GetComponentInChildren<DamageForPreSetupObjects>().Initialize(damage * damageMultiplier, attacker);
        g.GetComponentInChildren<RFX1_TransformMotion>().CollidesWith =~ LayerMask.GetMask(LayerMask.LayerToName(attacker.layer),"Ignore Raycast","Zone", "Dialogue");
        if (g.GetComponent<RFX1_Target>()) g.GetComponent<RFX1_Target>().Target = targetObject;
    }
    
}
