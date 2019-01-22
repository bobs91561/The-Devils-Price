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

    private int callNumber = 0;
    
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

    private void EffectAttack()
    {
        //Generate effect at casting point location
        GameObject g = Instantiate(effectToGenerate);
        effectGenerated = g;

        Vector3 castingPoint = attacker.GetComponent<SkillSet>().castingObject.transform.position;

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
        GameObject g = effectGenerated;
        g.GetComponentInChildren<DamageForPreSetupObjects>().gameObject.layer = attacker.layer;
        g.GetComponentInChildren<DamageForPreSetupObjects>().Initialize(damage, attacker);
        g.GetComponentInChildren<RFX1_TransformMotion>().CollidesWith = ~LayerMask.GetMask(LayerMask.LayerToName(attacker.layer), "Ignore Raycast", "Zone", "Dialogue");
        if (g.GetComponent<RFX1_Target>()) g.GetComponent<RFX1_Target>().Target = targetObject;
    }

    private void UseCustom()
    {
        GameObject g = effectGenerated;
        g.GetComponent<DamageForCustomObjects>().Initialize(damage, ~LayerMask.GetMask(LayerMask.LayerToName(attacker.layer), "Ignore Raycast", "Zone", "Dialogue"), attacker);
    }

    public override void End()
    {
        base.End();
        //Destroy(effectGenerated);
    }
    
}
