using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingCharacter : ThirdPersonCharacter
{
    private NavMeshAgent m_Agent;
    private Collider m_Collider;

    public float FlyingBaseOffset;
    public float FlyingHeight;
    public float FlyingRadius;

    private float RegularBaseOffset;
    private float RegularHeight;
    private float RegularRadius;

    // Start is called before the first frame update
    void Start()
    {
        RegularBaseOffset = m_Agent.baseOffset;
        RegularHeight = m_Agent.height;
        RegularRadius = m_Agent.radius;
    }

    public override void Move(Vector3 move, bool crouch, bool jump, bool restrictForward = false, bool sprint = false, bool dodge = false)
    {
        if (movementType == MovementType.REGULAR)
            base.Move(move, crouch, jump, restrictForward, sprint, dodge);
        else if(movementType == MovementType.AIRBORNE)
            AirborneMove(move);
    }

    /// <summary>
    /// 
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
        }
        else if (movementType == MovementType.REGULAR)
        {
            m_Agent.radius = RegularRadius;
            m_Agent.baseOffset = RegularBaseOffset;
            m_Agent.height = RegularHeight;
        }
    }
}
