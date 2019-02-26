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
    public override void UseAttack()
    {
        //Instantiate the block hitbox
        var go = Instantiate(objectToGenerate, m_BlockingObject.transform);
        //Initialize the hb
        go.GetComponent<BlockHitbox>().Initialize(attacker, m_BlockRating, m_BlockingWeapon);
    }

    public override void End()
    {
        base.End();
        Animator a = attacker.GetComponent<Animator>();
        if (!a) a = attacker.GetComponentInChildren<Animator>();
        if (a)
        {
            a.SetTrigger(m_SecondAnimKey);
        }
    }

    public override void Initialize(GameObject g)
    {
        base.Initialize(g);
        m_BlockingObject = skillSet.meleeObject;
        m_BlockingWeapon = m_BlockingObject.GetComponent<Weapon>();
        m_BlockRating = m_BlockingWeapon.GetDamage() * 0.75f;
    }
}
