using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies an explosive force to nearby rigidbodies. If created due to spell, does not affect caster layer.
/// </summary>
public class ExplosiveForce : MonoBehaviour
{
    public bool SpellEffect;
    public bool MajorReactOnly;

    public float radius = 5.0F;
    public float power = 10.0F;

    public float Damage;
    public LayerMask mask;

    private GameObject _mParent;

    public void Initialize(GameObject parent, float damage, LayerMask msk)
    {
        Damage = damage;
        mask = msk;
        _mParent = parent;
        Vector3 explosionPos = parent.transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius, mask);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb && !MajorReactOnly)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
            }

            HealthManager hm = hit.GetComponent<HealthManager>();
            ReactionManager rm = hit.GetComponent<ReactionManager>();

            if (hm)
            {
                hm.ModifyHealth(-Damage);
                hm.hitBy = parent;
                //hm.ReactMajor();
            }
            if(rm)
                rm.SpecialReact();
        }
    }
}
