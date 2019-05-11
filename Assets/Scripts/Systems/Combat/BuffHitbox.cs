using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHitbox : MonoBehaviour
{
    private float m_Duration;
    private float m_TimeSinceStart;
    private bool m_Grounded;
    private bool m_SingleMoment;
    private bool m_ReverseOnComplete;

    private GameObject m_Target;
    private SkillSet m_TargetSkillSet;
    private HealthManager m_TargetHealthManager;

    protected float m_BuffValue;

    public BuffType buffType;

    public virtual void Initialize(GameObject target, bool grounded, bool singlemoment, bool reverse, float value, float duration = 0f)
    {
        m_TimeSinceStart = 0f;
        m_Duration = duration;
        m_BuffValue = value;
        m_ReverseOnComplete = reverse;
        m_Target = target;

        m_TargetHealthManager = m_Target.GetComponent<HealthManager>();
        m_TargetSkillSet = m_Target.GetComponent<SkillSet>();

        if (singlemoment)
        {
            ApplyBuff();
            StartCoroutine(DestroyAfterTime());
            return;
        }
        else if (!grounded)
        {
            //If the effect is not grounded (does not have a trigger area), apply the effect over the duration
            StartCoroutine(ApplyOverTime());
        }
        else
            StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator ApplyOverTime()
    {
        while (m_TimeSinceStart < m_Duration)
        {
            ApplyBuff();
            m_TimeSinceStart += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(m_Duration);
        Destroy(gameObject);
    }

    protected virtual void ApplyBuff()
    {
        float val = m_SingleMoment ? m_BuffValue : m_BuffValue * Time.deltaTime;
        // Check bufftype
        // Apply val to proper attribute
        switch (buffType)
        {
            case BuffType.HEALTH:
                if (!m_TargetHealthManager) return;
                m_TargetHealthManager.ModifyHealth(val);
                break;
            case BuffType.DAMAGE:
                if (!m_TargetSkillSet) return;
                break;
            case BuffType.SPEED:
                break;
        }
    }

    protected virtual void ApplyBuff(GameObject g)
    {
        float val = m_SingleMoment ? m_BuffValue : m_BuffValue * Time.deltaTime;        

        // Check bufftype
        // Apply val to proper attribute
        switch (buffType)
        {
            case BuffType.HEALTH:
                HealthManager hm = g.GetComponent<HealthManager>();
                if (!hm) return;
                hm.ModifyHealth(val);
                break;
            case BuffType.DAMAGE:
                SkillSet set = g.GetComponent<SkillSet>();
                if (!set) return;
                break;
            case BuffType.SPEED:
                break;
        }
    }

    private void ReverseBuff()
    {
        // Check bufftype
        // Disapply the bonus from the buff
        switch (buffType)
        {
            case BuffType.HEALTH:
                break;
            case BuffType.DAMAGE:
                break;
            case BuffType.SPEED:
                break;
        }
    }

    public void ReceiveValue(float val)
    {
        m_BuffValue = val;
        ApplyBuff();
    }

    // When the effect is grounded to a spot
    #region Collision Logic

    public void OnDestroy()
    {
        if (!m_ReverseOnComplete) return;

        // Reverse the buff
        ReverseBuff();
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject g = collision.gameObject;
        ApplyBuff(g);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject g = other.gameObject;
        ApplyBuff(g);

    }

    void OnTriggerStay(Collider other)
    {
        GameObject g = other.gameObject;
        ApplyBuff(g);

    }

    void OnParticleCollision(GameObject other)
    {
        GameObject g = other.gameObject;
        ApplyBuff(g);
    }

    #endregion
}

public enum BuffType
{
    HEALTH, DAMAGE, SPEED
}
