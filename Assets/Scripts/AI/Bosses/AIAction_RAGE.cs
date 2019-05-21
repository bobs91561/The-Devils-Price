using UnityEngine;
using System.Collections;

public class AIAction_RAGE : AIAction
{
    public float Duration;
    public GameObject VisualEffect;
    private Rage m_Rage;
    private bool m_Enraged = false;
    private int m_RageTrigger = Animator.StringToHash("RageTrigger");

    public override bool ActionFeasible()
    {
        return !_skillSet.CheckAttack() && m_Enraged;
    }

    public override bool Tick()
    {
        if (waiting) return true;

        // Perform any animations
        // Instantiate any effects
        if (!isActive)
        {
            PerformVisualEffect();
            PerformAnimation();
            return true;
        }
        else if(!m_Enraged)
        {
            TriggerRage();
            return true;
        }
        return false;
    }

    private void PerformAnimation()
    {
        _animator.SetTrigger(m_RageTrigger);
        waiting = true;
    }

    private void PerformVisualEffect()
    {
        if (!VisualEffect) return;
        Instantiate(VisualEffect);
    }

    // Called outside by the Rage script
    public void TriggerRage()
    {
        m_Enraged = true;
        // Apply Bonuses
        // Start Coroutine
        m_Rage.BeginRage(Duration);
    }

    public void RemoveRage()
    {
        m_Rage.EndRage();
        m_Enraged = false;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        m_Rage = g.GetComponent<Rage>();
    }
}
