using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName ="AIAction/ChangeMode/HEALTH")]
public class ChangeMode_HEALTH : AIAction_CHANGEMODE
{
    private HealthManager m_HealthManager;

    protected override bool CheckCondition()
    {
        return false;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        m_HealthManager = g.GetComponent<HealthManager>();
    }
}
