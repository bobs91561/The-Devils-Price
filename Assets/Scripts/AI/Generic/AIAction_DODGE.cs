using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIAction/DODGE")]
public class AIAction_DODGE : AIAction {

    private float _timeSinceDodge;
    private ThirdPersonCharacter m_Character;

    private Attack m_PlayerAttack;
    private SkillSet m_PlayerSkillSet;

    private Vector3 m_position;
    private Vector3 n_position; //player position

    private Vector3 m_Direction;
    private NavMeshHit _hit;

    public float MinTimeBetweenDodge;
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
            return Dodge();
        }
        //return true if Dodge in progress
        //return false otherwise
        return ContinueDodge();
    }
    
    private bool Dodge()
    {        
        //Update timeSinceDodge
        _timeSinceDodge = 0f;
        //Choose a direction to dodge in
        bool choose = ChooseDodgeDirection();

        //Tell the AIController to dodge on move update
        if(choose)
        {
            m_Character.m_IsDodging = true;
            m_Character.transform.forward = m_Direction;
            Debug.Log("We did it Reddit");
        }
        return choose;
    }

    private bool ChooseDodgeDirection()
    {
        m_Direction = new Vector3();
        //Get the player's currentAttack
        m_PlayerAttack = m_PlayerSkillSet.currentAttack;
        if (!m_PlayerAttack) return false;

        //Get the range of the attack
        var forwardRange = m_PlayerAttack.maxForwardDistance;
        var areaRange = m_PlayerAttack.areaOfAttack;
        var closestToMe = m_Character.transform.position + m_Character.transform.forward * forwardRange;

        //Choose a direction that moves the AI out of range
        bool dodge_needed_right = NavMesh.Raycast(m_position, closestToMe + new Vector3(areaRange, 0, 0), out _hit,
            NavMesh.AllAreas);
        bool dodge_needed_left = NavMesh.Raycast(m_position, closestToMe + new Vector3(-areaRange, 0, 0), out _hit,
            NavMesh.AllAreas);
        bool dodge_needed_back = NavMesh.Raycast(m_position, closestToMe + new Vector3(0, 0, -areaRange), out _hit,
            NavMesh.AllAreas);
        bool dodge_needed_forward = NavMesh.Raycast(m_position, closestToMe + new Vector3(0, 0, areaRange), out _hit,
            NavMesh.AllAreas);
        if (dodge_needed_right)
        {
            m_Direction = new Vector3(1, 0, 0);
            return true;
        }
        else if (dodge_needed_left)
        {
            m_Direction = new Vector3(-1, 0, 0);
            return true;
        }
        else if (dodge_needed_back)
        {
            m_Direction = new Vector3(0, 0, -1);
            return true;
        }
        else if (dodge_needed_forward)
        {
            m_Direction = new Vector3(0, 0, 1);
            return true;
        }
        return false;
    }

    private bool ContinueDodge()
    {
        return m_Character.m_IsDodging;
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
        m_Character = g.GetComponent<ThirdPersonCharacter>();
        _timeSinceDodge = MinTimeBetweenDodge;
        m_PlayerSkillSet = AIActionDecider.Player.GetComponent<SkillSet>();
    }
}
