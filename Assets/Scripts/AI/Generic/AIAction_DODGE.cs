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
        if (IsPlayerPresent())
            m_PlayerSkillSet = AIActionDecider.Player.GetComponent<SkillSet>();
        //Debug.Log("Action Feasible:" + (decider.combat && !_skillSet.CheckAttack() && AIActionDecider.Player.GetComponent<SkillSet>().CheckAttack() && CheckTime()));
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
            g.GetComponent<AIController>().Dodge = true;
        }
        return choose;
    }

    private bool ChooseDodgeDirection()
    {
        m_position = g.transform.position;
        m_Direction = new Vector3();
        //Get the player's currentAttack
        m_PlayerAttack = m_PlayerSkillSet.currentAttack;
        if (!m_PlayerAttack) return false;

        //Get the range of the attack
        var forwardRange = m_PlayerAttack.maxForwardDistance;
        var areaRange = m_PlayerAttack.areaOfAttack;
        var closestToMe = m_Character.transform.position + m_Character.transform.forward * forwardRange + m_Character.transform.forward * areaRange;
        
        var backVector = m_position + new Vector3(0, 0, -3);
        var leftVector = m_position + new Vector3(-3, 0, 0);
        var rightVector = m_position + new Vector3(3, 0, 0);
        var forwardVector = m_position + new Vector3(0, 0, 3);
        

        //Choose a direction that moves the AI out of range
        //This code is fucking nonsense but we'll hold onto it because why not 
        /*bool ableDodgeRight = NavMesh.Raycast(m_position, closestToMe + new Vector3(areaRange, 0, 0), out _hit,
            NavMesh.AllAreas);
        Debug.Log("Right " + ableDodgeRight);
        bool ableDodgeLeft = NavMesh.Raycast(m_position, closestToMe + new Vector3(-areaRange, 0, 0), out _hit,
            NavMesh.AllAreas);
        bool ableDodgeBack = NavMesh.Raycast(m_position, closestToMe + new Vector3(0, 0, -areaRange), out _hit,
            NavMesh.AllAreas);
        bool ableDodgeForward = NavMesh.Raycast(m_position, closestToMe + new Vector3(0, 0, areaRange), out _hit,
            NavMesh.AllAreas);
        Debug.Log("Right " + ableDodgeRight);
        Debug.Log("Left " + ableDodgeLeft);
        Debug.Log("Back " + ableDodgeBack);
        Debug.Log("Forward " + ableDodgeForward);*/
        bool ableDodgeBack = NavMesh.Raycast(m_position, backVector, out _hit,
            NavMesh.AllAreas);
        bool ableDodgeLeft = NavMesh.Raycast(m_position, leftVector, out _hit,
            NavMesh.AllAreas);
        bool ableDodgeRight = NavMesh.Raycast(m_position, rightVector, out _hit,
            NavMesh.AllAreas);
        bool ableDodgeForward = NavMesh.Raycast(m_position, forwardVector, out _hit,
            NavMesh.AllAreas);
        /*Debug.Log("Right " + ableDodgeRight);
        Debug.Log("Left " + ableDodgeLeft);
        Debug.Log("Back " + ableDodgeBack);
        Debug.Log("Forward " + ableDodgeForward);
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
        }*/
        float maxDistanceAway = 0f;
        if (!ableDodgeBack && Vector3.Distance(backVector, closestToMe) > maxDistanceAway)
        {
            maxDistanceAway = Vector3.Distance(backVector, closestToMe);
            m_Direction = new Vector3(0, 0, -1);
        }
        if (!ableDodgeLeft && Vector3.Distance(leftVector, closestToMe) > maxDistanceAway)
        {
            maxDistanceAway = Vector3.Distance(leftVector, closestToMe);
            m_Direction = new Vector3(-1, 0, 0);
        }
        if (!ableDodgeForward && Vector3.Distance(forwardVector, closestToMe) > maxDistanceAway)
        {
            maxDistanceAway = Vector3.Distance(forwardVector, closestToMe);
            m_Direction = new Vector3(0, 0, 1);
        }
        if (!ableDodgeRight && Vector3.Distance(forwardVector, closestToMe) > maxDistanceAway)
        {
            m_Direction = new Vector3(1, 0, 0);
        }

        if (maxDistanceAway.Equals(0f))
            return false;
        return true;
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
        // m_PlayerSkillSet = AIActionDecider.Player.GetComponent<SkillSet>();
    }


}
