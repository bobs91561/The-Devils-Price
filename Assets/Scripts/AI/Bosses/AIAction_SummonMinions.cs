using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Action for boss-level entities with followings
/// Uses an attack separate from attack controller, but still triggers with skill set
/// </summary>
[CreateAssetMenu(menuName ="AIAction/Special/SummonMinions")]
public class AIAction_SummonMinions : AIAction {

    public int MaxCalls;
    public float[] ThresholdLevels;
    public float TimeBetweenCalls;
    public ActionDependency DependentOn;

    public Attack SummonMinionAttack;

    private int _mCallNumber;
    private float _mTimeSinceCall;

    private SkillSet _mSkillSet;

    /// <summary>
    /// Feasible if MaxCalls not exceeded, Threshold Level of Dependency is met, combat is active
    /// </summary>
    /// <returns></returns>
    public override bool ActionFeasible()
    {
        return decider.combat && _mCallNumber < MaxCalls && CheckDependence();
    }

    public override bool Tick()
    {
        //If isAttacking, wait and return true
        if (_mSkillSet.CheckAttack()) return true;
        //when not attacking, start the attack
        //if the current attack of the skill set is summonminionattack,
        //  update the time and call number fields, otherwise startattack
        if(_mSkillSet.currentAttack == SummonMinionAttack)
        {
            _mCallNumber++;
            _mTimeSinceCall = 0f;
        }
        else
            _mSkillSet.StartAttack(SummonMinionAttack);

        return true;
    }

    private bool CheckDependence()
    {
        bool dependence = false;
        HealthManager hm;
        switch (DependentOn)
        {
            //If the AI Health drops below a threshold
            case ActionDependency.AIHealth:
                hm = decider.AI.GetComponent<HealthManager>();
                dependence = BelowThreshold(hm.Health, hm.maxHealth);
                break;
            //If the player Health drops below a threshold
            case ActionDependency.PlayerHealth:
                hm = AIActionDecider.Player.GetComponent<HealthManager>();
                dependence = AboveThreshold(hm.Health, hm.maxHealth);
                break;
            //If the time between the last call is greater than the minimum
            case ActionDependency.Time:
                break;
        }
        return dependence;
    }

    private bool AboveThreshold(float current, float max)
    {
        float threshold = ThresholdLevels[_mCallNumber];
        return current / max >= threshold;
    }

    private bool BelowThreshold(float current, float max)
    {
        float threshold = ThresholdLevels[_mCallNumber];
        return current / max <= threshold;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _mCallNumber = 0;
        _mSkillSet = obj.GetComponent<SkillSet>();
    }

}

public enum ActionDependency
{
    AIHealth, PlayerHealth, Time
}
