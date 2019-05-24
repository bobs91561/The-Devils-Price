using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AIAction/BLOCK")]
public class AIAction_BLOCK : AIAction
{
    public float MinimumTimeBetweenBlocks = 2f;
    private float m_TimeOfLastBlock;

    private Attack m_BlockAttack;

    private float m_MaxDistance;
    private Transform m_Player;
    private SkillSet m_PlayerSkillSet;

    public override bool ActionFeasible()
    {
        if (_skillSet.CheckBlock()) return true;

        // Check the AI is not attacking, the player is with a melee and the player is in a valid range
        return !_skillSet.CheckAttack() && CheckPlayerAttack() && CheckPlayerDistance() && CheckTimeDifference();
    }

    public override bool Tick()
    {
        if (_skillSet.CheckBlock()) return true;

        // Tell Skill set to block if not previously activated
        if (!isActive)
        {
            _skillSet.StartAttack(m_BlockAttack);
            return true;
        }

        // Block was force ended
        return false;
    }

    private bool CheckTimeDifference()
    {
        return Time.time - m_TimeOfLastBlock >= MinimumTimeBetweenBlocks;
    }

    private bool CheckPlayerDistance()
    {
        return Vector3.Distance(g.transform.position, m_Player.position) <= m_MaxDistance;
    }

    private bool CheckPlayerAttack()
    {
        return m_PlayerSkillSet.CheckAttack() && m_PlayerSkillSet.currentAttack.GetType() == typeof(Melee);
    }

    public override void SetActive()
    {
        if (isActive && _skillSet.CheckBlock())
        {
            _skillSet.EndAttack();
            m_TimeOfLastBlock = Time.time;
        }
        base.SetActive();
    }

    public override void Initialize(GameObject obj = null)
    {
        base.Initialize(obj);
        m_BlockAttack = _skillSet.BlockAttack;
        m_Player = GameManager.Player.transform;
        m_PlayerSkillSet = GameManager.Player.GetComponent<SkillSet>();
    }
}
