using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Systems.Combat;

public class AIAttackController : AttackController {

    public static GameObject Player;
    public static GameObject PlayerCenter;

    private float AverageOfDistances;

    public bool ConsiderMove;
    public float AverageMaxDistance;

    public float MinimumAttackTimeDifference = 2f;

    // Use this for initialization
    void Start () {
        if (Player == null) Player = GameObject.Find("Player");
        if (PlayerCenter == null && Player) PlayerCenter = Player.GetComponent<SkillSet>().characterCenter;
        AverageMaxDistance = 0;
        enabled = false;
        ConsiderMove = false;
	}

    public override void SetAttacks()
    {
        Initialize();
        foreach (Attack a in _attacks)
        {
            //_cooldowns.Add(a, a.coolDown);
            AverageMaxDistance += a.maxForwardDistance;
        }
        SetUpCooldowns();
        AverageMaxDistance = AverageMaxDistance / _attacks.Count;

    }

    public bool ChooseAttack()
    {
        if (!enabled) enabled = true;
        //Get the feasible attacks
        List<Attack> feasible = GetFeasibleAttacks();
        float Max = Mathf.NegativeInfinity, curr = 0f;
        Attack best = null;

        //Modify the AI rotation to be facing player
        transform.forward = Player.transform.position - transform.position;

        //Analyze for best attacks
        //Do best attack
        foreach(Attack a in feasible)
        {
            curr = AnalyzeAttack(a);
            if(curr>=Max)
            {
                Max = curr;
                best = a;
            }
        }

        if(Max == Mathf.NegativeInfinity)
        {
            ConsiderMove = true;
            return false;
        }
        
        //Higher value = better attack
        //Perform best attack
        if (best != null)
        {
            _skillSet.StartAttack(best);
            if (!PlayerCenter) PlayerCenter = GameObject.Find("Player").GetComponent<SkillSet>().characterCenter;
            _skillSet.TargetThis(PlayerCenter);
            //Update cooldown
            SendAttack(best);
            ConsiderMove = false;
            return true;
        }
        ConsiderMove = true;
        return false;
    }

    private float AnalyzeAttack(Attack a)
    {
        //Check for any resource requirements (stamina, soulStrength,etc)
        //Weigh against:
        //Potential damage done vs player health, cost vs available resources, successful hit possibility
        float damageAnalysis = a.damage - Player.GetComponent<HealthManager>().Health;
        float cost = a.soulStrengthRequired - SoulStrength;
        float speed = a.attackSpeed - a.coolDown;

        float success = 0f;
        //Success possibility first checks that the Player is within the max distance
        //if not, succesful hit it weighed infinitely negative
        //if yes, increase the success possibility with the area covered by the attack
        if (Vector3.Distance(transform.position, Player.transform.position) <= a.maxForwardDistance)
        {
            success += Vector3.Distance(transform.position, Player.transform.position) - a.maxForwardDistance + a.areaOfAttack;
        }
        else
            return Mathf.NegativeInfinity;

        return damageAnalysis + cost + success + speed;
    }

    private List<Attack> GetFeasibleAttacks()
    {
        List<Attack> attacks = new List<Attack>();
        foreach(Attack a in _attacks)
        {
            if (a.CheckRequirements(_cooldowns[a], 0f))
                attacks.Add(a);
        }
        return attacks;
    }

    // TODO Change soulRequirement from 0 to available
    public bool AttacksArePossible()
    {
        bool available = false;
        foreach (Attack a in _attacks)
        {
            try
            {
                if (a.CheckRequirements(_cooldowns[a], 0f))
                {
                    ConsiderMove = false;
                    return true;
                }
            }
            catch(KeyNotFoundException e)
            {
                Debug.Log(a + " " + gameObject.name);
            }
        }
        ConsiderMove = true;
        return available;
    }

    #region Levelling Settings
    public void ChangeAttackRate(float multiplier)
    {
        MinimumAttackTimeDifference -= MinimumAttackTimeDifference*multiplier;
    }
    #endregion
}
