using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingCharacter : ThirdPersonCharacter
{
    private NavMeshAgent m_Agent;
    private Collider m_Collider;

    private int m_FlightMode = Animator.StringToHash("FlightMode");
    private int m_FlightLayer;
    private int m_BaseLayer;
    private bool m_UsesLayers;
    private Animator m_SecondaryAnimator;

    public float FlyingBaseOffset;
    public float FlyingHeight;
    public float FlyingRadius;

    private float RegularBaseOffset;
    private float RegularHeight;
    private float RegularRadius;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_Agent = GetComponent<NavMeshAgent>();
        RegularBaseOffset = m_Agent.baseOffset;
        RegularHeight = m_Agent.height;
        RegularRadius = m_Agent.radius;

        if (FlyingHeight == 0) FlyingHeight = RegularHeight;
        if (FlyingRadius == 0) FlyingRadius = RegularRadius;
        if (FlyingBaseOffset == 0) FlyingBaseOffset = RegularBaseOffset;

        var layers = m_Animator.layerCount;
        if (layers > 1)
        {
            m_FlightLayer = m_Animator.GetLayerIndex("Flying Layer");
            m_BaseLayer = m_Animator.GetLayerIndex("Base Layer");
            m_UsesLayers = true;
        }
        else
        {
            m_UsesLayers = false;
            m_SecondaryAnimator = GetComponentsInChildren<Animator>()[1];
        }
    }

    public override void Move(Vector3 move, bool crouch, bool jump, bool restrictForward = false, bool sprint = false, bool dodge = false)
    {
        if (movementType == MovementType.REGULAR)
            base.Move(move, crouch, jump, restrictForward, sprint, dodge);
        else if(movementType == MovementType.AIRBORNE)
            AirborneMove(move);
    }

    /// <summary>
    /// Handles the movement of an airborne character
    /// </summary>
    /// <param name="move"></param>
    private void AirborneMove(Vector3 move)
    {

    }

    public override void ChangeMovementType(MovementType type)
    {
        base.ChangeMovementType(type);
        if(movementType == MovementType.AIRBORNE)
        {
            m_Agent.radius = FlyingRadius;
            m_Agent.baseOffset = FlyingBaseOffset;
            m_Agent.height = FlyingHeight;

            if (m_UsesLayers)
            {
                m_Animator.SetLayerWeight(m_FlightLayer, 1);
                m_Animator.SetLayerWeight(m_BaseLayer, 0);
            }
            else
            {
                m_SecondaryAnimator.SetBool(m_FlightMode, true);
            }
        }
        else if (movementType == MovementType.REGULAR)
        {
            m_Agent.radius = RegularRadius;
            m_Agent.baseOffset = RegularBaseOffset;
            m_Agent.height = RegularHeight;

            if (m_UsesLayers)
            {
                m_Animator.SetLayerWeight(m_FlightLayer, 0);
                m_Animator.SetLayerWeight(m_BaseLayer, 1);
            }
            else
            {
                m_SecondaryAnimator.SetBool(m_FlightMode, false);
            }
        }
    }
}
