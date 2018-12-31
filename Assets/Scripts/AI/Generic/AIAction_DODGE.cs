using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/DODGE")]
public class AIAction_DODGE : AIAction {

    private NavMeshAgent _agent;
    private SkillSet _skillSet;
    private float _timeSinceDodge;

    public float MinTimeBetweenDodge;
    public string DodgeAnim;
    public float TimeToCompleteDodge;

    public override bool ActionFeasible()
    {
        //Is feasible if the AI is not attacking, the player is attacking,
        //  and the dodge time is not less than the threshold
        UpdateTime();
        return decider.combat && !_skillSet.CheckAttack() && AIActionDecider.Player.GetComponent<SkillSet>().CheckAttack() && CheckTime();
    }

    public override bool Tick()
    {
        //return true if not Dodging yet
        if (!isActive)
        {
            Dodge();
            return true;
        }
        //return true if Dodge in progress
        //return false otherwise
        return ContinueDodge();
    }
    
    private void Dodge()
    {
        //Trigger Dodge Animation
        decider.AI.GetComponent<Animator>().SetTrigger(DodgeAnim);
        
        //Update timeSinceDodge
        _timeSinceDodge = 0f;
    }

    private bool ContinueDodge()
    {
        return _timeSinceDodge >= TimeToCompleteDodge;
    }

    private void UpdateTime()
    {
        _timeSinceDodge += Time.deltaTime;
    }

    private bool CheckTime()
    {
        return _timeSinceDodge >= MinTimeBetweenDodge;
    }

    public override void SetActive()
    {
        //Remove any path the agent may have had
        if (_agent.hasPath) _agent.ResetPath();
        base.SetActive();
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        _skillSet = g.GetComponent<SkillSet>();
        _timeSinceDodge = MinTimeBetweenDodge;
        _agent = g.GetComponent<NavMeshAgent>();
    }
}
