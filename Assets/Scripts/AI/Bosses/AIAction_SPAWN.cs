using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "AIAction/SPAWN")]

/// <summary>
/// This class is used to determine if the spawn point chosen is valid, and spawns enemies of the appropriate type
/// </summary>
public class AIAction_SPAWN : AIAction
{
    public enum ActionDependency
    {
        AIHealth, PlayerHealth, Time, Ally
    }

    [HideInInspector] public GameObject spawner;
    public float radius;
    public GameObject PortalType;
    public GameObject EnemyType;
    public float[] ThresholdLevels;
    public float WaitTime; //time between portal spawn and enemy spawn
    //public Attack SpawnAttack;
    public AudioClip audioToPlay;
    public string animationKey;


    public AIAction Spawn;
    public float mintime; //minimum time until first spawn
    public ActionDependency DependentOn;
    private GameObject SpawnPoint;

    private bool _noPortal;
    private float _time;
    private int _mCallNumber;
    private SkillSet _mSkillSet;
    private float _mTimeSinceCall;
    private HealthManager AIhm;
    private HealthManager Playerhm;
    private float lastCallTime;
    private int m_SpawnId;
    private bool m_AnimationCalled;

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        spawner = g;
        _mCallNumber = 0;
        _mSkillSet = obj.GetComponent<SkillSet>();
        AIhm = decider.AI.GetComponent<HealthManager>();
        lastCallTime = 0;
        _mTimeSinceCall = 0;
        _noPortal = true;
        m_SpawnId = Animator.StringToHash("Spawn");
        m_AnimationCalled = false;
    }

    public virtual void TriggerAnimation()
    {
        Animator s = g.GetComponent<Animator>();
        if (!s) s = g.GetComponentInChildren<Animator>();
        if (s)
            s.SetTrigger(m_SpawnId);
        m_AnimationCalled = true;
    }


    public override bool Tick()
    {
        //If isAttacking, wait and return true
        //if (_mSkillSet.CheckAttack()) return true;
        //when not attacking, start the spawn
        //  update the time and call number fields
        if (!m_AnimationCalled)
        {
            TriggerAnimation();
            return true;
        }

        if (_noPortal)
        {
            foreach (var t in decider.spawnPoints)
                Instantiate(PortalType, t.transform.position, Quaternion.identity);
            _mCallNumber++;
            _mTimeSinceCall = 0f;
            _noPortal = false;
            return true;
        }
        if (_mTimeSinceCall > WaitTime)
        {
            _mTimeSinceCall = 0f;
            foreach (var t in decider.spawnPoints)
            {
                Instantiate(EnemyType, t.transform.position, t.transform.rotation);
            }
            return true;
        }

        //else
        //    _mSkillSet.StartAttack(SpawnAttack);
        return false;
    }

    private bool CheckDependence()
    {
        bool dependence = false;
        switch (DependentOn)
        {
            //If the AI Health drops below a threshold
            case ActionDependency.AIHealth:
                dependence = BelowThreshold(AIhm.Health, AIhm.maxHealth);
                break;
            //If the player Health drops below a threshold
            case ActionDependency.PlayerHealth:
                Playerhm = AIActionDecider.Player.GetComponent<HealthManager>();
                dependence = AboveThreshold(Playerhm.Health, Playerhm.maxHealth);
                break;
            //If the time between the last call is greater than the minimum
            case ActionDependency.Time:
                dependence = (Time.time - lastCallTime) > mintime;
                if (dependence) lastCallTime = Time.time;
                break;
            //If there are more allies needed before spawn
            case ActionDependency.Ally:
                dependence = AllyThreshold();
                break;
        }
        return dependence;
    }

    private bool AllyThreshold()
    {
        if (_mCallNumber > ThresholdLevels.Length) return false;
        float threshold = ThresholdLevels[_mCallNumber];
        Collider[] cs = Physics.OverlapSphere(_agent.transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
        int allies = 0;
        foreach (Collider c in cs)
        {
            allies++;
        }

        return threshold >= allies;
    }

    private bool AboveThreshold(float current, float max)
    {
        if (_mCallNumber > ThresholdLevels.Length) return false;
        float threshold = ThresholdLevels[_mCallNumber];
        return current / max >= threshold;
    }

    private bool BelowThreshold(float current, float max)
    {
        if (_mCallNumber > ThresholdLevels.Length) return false;
        float threshold = ThresholdLevels[_mCallNumber];
        return current / max <= threshold;

    }

    public override bool ActionFeasible()
    {
        _mTimeSinceCall++;
        if (!decider.combat || !CheckDependence()) return false;
        // check to make sure all spawnpoints are within the radius
        if (decider.spawnPoints.Any(s => Vector3.Distance(s.transform.position, _agent.transform.position) > radius))
        {
            return false;
        }

        return true;
    }

    public override void SetActive()
    {
        base.SetActive();
        m_AnimationCalled = false;
    }

}
