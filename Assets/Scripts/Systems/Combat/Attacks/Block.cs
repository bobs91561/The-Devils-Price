using System.Collections;
using System.Collections.Generic;
using Systems.Combat;
using UnityEngine;

[CreateAssetMenu(menuName ="Attacks/Block")]
public class Block : Attack
{
    private Vector3 m_HBLocation;
    private GameObject m_BlockingObject;
    private Weapon m_BlockingWeapon;
    private float m_BlockRating;
    private int m_SecondAnimKey = Animator.StringToHash("FinishBlock");
    private int m_BlockBool = Animator.StringToHash("isBlocking");
    public override void UseAttack()
    {
        Animator a = attacker.GetComponent<Animator>();
        if (!a) a = attacker.GetComponentInChildren<Animator>();
        if (a)
        {
            a.SetBool(m_BlockBool, true);
            a.ResetTrigger(animationKey);
        }
        //Instantiate the block hitbox
        var go = Instantiate(objectToGenerate, m_BlockingObject.transform);
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
        //Initialize the hb
        go.GetComponent<BlockHitbox>().Initialize(attacker, m_BlockRating, m_BlockingWeapon);
        objectGenerated = go;
    }

    public override void End()
    {
        Animator a = attacker.GetComponent<Animator>();
        if (!a) a = attacker.GetComponentInChildren<Animator>();
        if (a)
        {
            a.SetBool(m_BlockBool, false);
        }
        base.End();
    }

    public override void Initialize(GameObject g)
    {
        base.Initialize(g);
        m_BlockingObject = skillSet.meleeObject;
        m_BlockingWeapon = m_BlockingObject.GetComponent<Weapon>();
        m_BlockRating = m_BlockingWeapon.GetDamage() * 0.75f;
        objectToGenerate = skillSet.BlockHitboxToGenerate;
    }
}
