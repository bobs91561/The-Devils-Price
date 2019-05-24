using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName ="AIAction/ChangeMode/HEALTH")]
public class ChangeMode_HEALTH : AIAction_CHANGEMODE
{
    private HealthManager m_HealthManager;
    private float m_MaxHealth;
    private float m_ThresholdValue;

    protected override bool CheckCondition()
    {
        return m_HealthManager.Health <= m_ThresholdValue;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        m_HealthManager = g.GetComponent<HealthManager>();
        m_MaxHealth = m_HealthManager.maxHealth;
        if (Threshold > 1) Threshold /= 100;
        m_ThresholdValue = Threshold * m_MaxHealth;
    }

    protected override void OnChangeMode()
    {
        m_MaxHealth = m_ThresholdValue;
    }
}
