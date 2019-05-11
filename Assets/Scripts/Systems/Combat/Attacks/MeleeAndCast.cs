using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Systems.Combat;
using Devdog.InventoryPro.UnityStandardAssets;
/// <summary>
/// Melee hitbox and generated effect. Attack base objectToGenerate is hitbox.
/// </summary>
[CreateAssetMenu(menuName ="Attacks/MeleeAndCast")]
public class MeleeAndCast : Melee
{
    public GameObject effectToGenerate;
    public GameObject effectGenerated;

    public bool effectDelayed;

    public bool effectGrounded;

    public bool effectAttachedToMelee;

    private int callNumber = 0;
    private CastingObject cast;
    private GameObject castingObject;
    private float damageMultiplier;

    /// <summary>
    /// Always calls the base melee attack first, then instantiates the effect.
    /// In case where the effect is delayed, the second call to this method will generate the effect, otherwise,
    ///     both melee and effect are done simultaeneously.
    /// </summary>
    public override void UseAttack()
    {
        if (effectDelayed)
            switch (callNumber)
            {
                case 0:
                    base.UseAttack();
                    callNumber++;
                    break;
                case 1:
                    EffectAttack();
                    callNumber = 0;
                    break;

            }
        else
        { 
            //Call the base melee attack
            base.UseAttack();
            EffectAttack();
        }
        
        
    }

    private void SetTransform()
    {
        Vector3 castingPoint = castingObject.transform.position;

        Vector3 attackerFwd = attacker.transform.forward;
        Vector3 attackerCtr = attacker.transform.position + attackerFwd * maxForwardDistance * .75f;
        attackerCtr.y = castingPoint.y;
        Vector3 rotated = (attackerCtr - castingPoint).normalized;
        effectGenerated.transform.position = !effectGrounded ? castingPoint : attacker.transform.position;
        effectGenerated.transform.forward = rotated;
        effectGenerated.layer = attacker.layer;

        if (effectGenerated.GetComponent<RFX1_Target>()) effectGenerated.GetComponent<RFX1_Target>().Target = targetObject;
    }

    private void SetTransformToMelee()
    {
        Vector3 castingPoint = meleeObject.transform.position;

        Vector3 attackerFwd = attacker.transform.forward;
        Vector3 attackerCtr = attacker.transform.position + attackerFwd * maxForwardDistance * .75f;
        attackerCtr.y = castingPoint.y;
        Vector3 rotated = (attackerCtr - castingPoint).normalized;

        effectGenerated.transform.position = castingPoint;
        effectGenerated.transform.forward = rotated;

        effectGenerated.layer = attacker.layer;

        if (effectGenerated.GetComponent<RFX1_Target>()) effectGenerated.GetComponent<RFX1_Target>().Target = targetObject;
    }

    private void EffectAttack()
    {
        damageMultiplier = skillSet.CastingDamage;

        //Generate effect at casting point location
        GameObject g = Instantiate(effectToGenerate);
        effectGenerated = g;

        if (effectAttachedToMelee) SetTransformToMelee();
        else SetTransform();
        

        if (g.GetComponentInChildren<DamageForPreSetupObjects>()) UsePreSetup();
        else UseCustom();
    }

    private void UsePreSetup()
    {
        GameObject g = objectGenerated;
        g.GetComponentInChildren<DamageForPreSetupObjects>().gameObject.layer = attacker.layer;
        g.GetComponentInChildren<DamageForPreSetupObjects>().Initialize(damage * damageMultiplier, attacker);
        g.GetComponentInChildren<RFX1_TransformMotion>().CollidesWith = ~LayerMask.GetMask(LayerMask.LayerToName(attacker.layer), "Ignore Raycast", "Zone", "Dialogue");

    }

    private void UseCustom()
    {
        GameObject g = objectGenerated;
        g.GetComponent<DamageForCustomObjects>().Initialize(damage * damageMultiplier, ~LayerMask.GetMask(LayerMask.LayerToName(attacker.layer), "Ignore Raycast", "Zone", "Dialogue"), attacker);
    }

    public override void End()
    {
        base.End();
        //Destroy(effectGenerated);
    }

    public override void Initialize(GameObject g)
    {
        base.Initialize(g);
        castingObject = skillSet.castingObject;
        cast = castingObject.GetComponent<CastingObject>();
    }

}
