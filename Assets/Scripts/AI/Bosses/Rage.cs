using UnityEngine;
using System.Collections;

public class Rage : MonoBehaviour
{
    public float DamageThreshold;
    public float MaxTrackTime = 5f;

    AIActionDecider m_Decider;
    AIAction_RAGE m_RageAction;

    HealthManager m_HealthManager;

    float m_PreviousValue;
    float m_Difference;
    float m_TotalDamageTracked;

    bool m_Tracking = false;
    bool m_WaitToTrack = false;
    

    // Use this for initialization
    void Start()
    {
        m_Decider = GetComponent<AIActionDecider>();
        m_HealthManager = GetComponent<HealthManager>();        
    }

    void FindRageAction()
    {
        var actions = m_Decider.actions;
        foreach(AIAction a in actions)
        {
            if(a.GetType() == typeof(AIAction_RAGE))
            {
                m_RageAction = (AIAction_RAGE)a;
                return;
            }
        }
    }

    public void HealthChanged()
    {
        if (m_WaitToTrack) return;
        m_Difference = m_PreviousValue - m_HealthManager.Health;
        TrackChanges();
    }

    private void TrackChanges()
    {
        m_TotalDamageTracked += m_Difference;
        if (!m_Tracking) BuildRage();
        m_Tracking = true;
    }

    IEnumerator BuildRage()
    {
        StartCoroutine("TrackTime");
        // Wait until damage exceeds threshold => trigger rage
        yield return new WaitUntil(()=> m_TotalDamageTracked >= DamageThreshold);
        TriggerRage();
    }

    IEnumerator TrackTime()
    {
        yield return new WaitForSeconds(MaxTrackTime);
        StopCoroutine("BuildRage");
        m_TotalDamageTracked = 0f;
    }

    private void TriggerRage()
    {
        if (!m_RageAction) FindRageAction();
        m_RageAction.TriggerRage();
        m_WaitToTrack = true;
    }

    public void BeginRage(float duration)
    {
        StartCoroutine("BeginRage", duration);
    }

    IEnumerator Raging(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_RageAction.RemoveRage();
    }

    public void EndRage()
    {
        StartCoroutine(WaitToTrack());
    }

    IEnumerator WaitToTrack()
    {
        yield return new WaitForSeconds(10f);
        m_WaitToTrack = false;
    }
    
}
