using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Systems.Combat;

[RequireComponent(typeof(Reputable))]
public class RankResponse : MonoBehaviour
{
    private Reputable m_Reputable;
    private int m_Rank;

    private HealthManager m_HealthManager;
    private AttackController m_AttackController;
    private SkillSet m_SkillSet;

    private float m_healthStep = 100f;
    private float m_damageStep = 0.5f;

    private bool m_AI = false;
    // Start is called before the first frame update
    void Start()
    {
        m_Reputable = GetComponent<Reputable>();
        m_HealthManager = GetComponent<HealthManager>();
        m_AttackController = GetComponent<AttackController>();
        m_SkillSet = GetComponent<SkillSet>();

        m_AI = (GetComponent<AIController>());
        AddToRankUpEvent();
    }

    private void OnDestroy()
    {
        RemoveFromRankUpEvent();
    }

    private void AddToRankUpEvent()
    {
        EventManager.RankUp += ModifyDamage;
        EventManager.RankUp += ModifyHealth;
        if(m_AI)
        {
            EventManager.RankUp += ForceRankUp;
        }
    }

    private void RemoveFromRankUpEvent()
    {
        EventManager.RankUp -= ModifyDamage;
        EventManager.RankUp -= ModifyHealth;
        if (m_AI)
        {
            EventManager.RankUp -= ForceRankUp;
        }
    }

    public void RankUp()
    {
        //Character Ranked Up
        m_Rank = m_Reputable.CurrentRank;

        //If Player, trigger event
        if (!m_AI) EventManager.IncreaseRank();
        else
        {
            ModifyDamage();
            ModifyHealth();
        }
    }

    private void ModifyHealth()
    {
        var health = m_healthStep * m_Rank;
        m_HealthManager.ChangeMaxHealth(health);
    }

    private void ModifyDamage()
    {
        var multiplier = m_damageStep * m_Rank;
        m_SkillSet.ModifyDamage(multiplier);
    }
    
    private void IncreaseReputation()
    {
        m_Reputable.IncreaseReputation(m_Reputable.Reputation * 0.1f * m_Rank);
    }

    private void ForceRankUp()
    {
        m_Reputable.ForceRankUp();
    }
}
