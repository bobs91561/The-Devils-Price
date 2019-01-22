using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This action is the base decision for the AI attacking. It checks that the player is in view,
///     the overall cooldown is not being violated, and that there are successful possibilities for attacks.
/// </summary>
[CreateAssetMenu(menuName ="AIAction/ATTACK")]
public class AIAction_ATTACK : AIAction {
    public float maxDistance;
    public float minCoolDownTime;
    public float TimeSinceLastAttack;
    public bool PlayerSpotted;
    private AIAttackController _attackController;

    public override bool ActionFeasible()
    {
        if (!IsPlayerPresent()) return false;
        UpdateTime();
        if (!CheckPlayerDeath())
        {
            decider.ExitCombat();
            return false;
        }
        return (!decider.Friendly && TimeSinceLastAttack >= minCoolDownTime && PlayerVisible() && AttacksArePossible()) || _skillSet.CheckAttack();
    }

    public override bool Tick()
    {
        //Update the decider's combat movement
        decider.combatApproach -= Time.deltaTime;

        //return true if the AI isAttacking
        if (_skillSet.CheckAttack())
        {
            TimeSinceLastAttack = 0f;
            return true;
        }

        //if not isAttacking, chooseAttack with AttackController
        //return whether attack was chosen
        bool b = _attackController.ChooseAttack();
        return b;
    }
    //TODO Check if the player is visible to the AI
    private bool PlayerVisible()
    {
        Vector3 targetDir = AIActionDecider.Player.transform.position - g.transform.position;
        float angleToPlayer = (Vector3.Angle(targetDir, g.transform.forward));
        float distanceToPlayer = Vector3.Distance(AIActionDecider.Player.transform.position, g.transform.position);

        //Is the player in the AI's FOV?
        if (angleToPlayer >= -90 && angleToPlayer <= 90 && distanceToPlayer <= maxDistance)
        {
            //Is the player visible to the AI (not being concealed)?
            //Check player visibility
            PlayerSpotted = true;
            decider.EnterCombat();

            return true;
        }

        if(decider.combat) decider.ExitCombat();
        return false;
    }

    private bool AttacksArePossible()
    {
        //Checks with the AIAttackController that there are feasible attacks
        return _attackController.AttacksArePossible();
    }

    private void UpdateTime()
    {
       TimeSinceLastAttack += Time.deltaTime;
    }

    private bool CheckPlayerDeath()
    {
        return AIActionDecider.Player.GetComponent<HealthManager>().isAlive;
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        PlayerSpotted = false;
        _attackController = g.GetComponent<AIAttackController>();
        minCoolDownTime = g.GetComponent<AIController>().MinimumAttackTimeDifference;
        TimeSinceLastAttack = minCoolDownTime;
    }

    /*public override void SetActive()
    {
        if (isActive) _animator.SetFloat("Forward", 0f);
        base.SetActive();
    }*/
}
