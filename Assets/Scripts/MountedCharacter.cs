using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MountedCharacter : ThirdPersonCharacter
{
    private NavMeshAgent m_Agent;
    private Collider m_Collider;

    private int m_FlightMode = Animator.StringToHash("RidingMode");
    private int m_FlightLayer;
    private int m_BaseLayer;
    private bool m_UsesLayers;
    private Animator m_SecondaryAnimator;

    public float RidingBaseOffset;
    public float RidingHeight;
    public float RidingRadius;

    private float RegularBaseOffset;
    private float RegularHeight;
    private float RegularRadius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Move(Vector3 move, bool crouch, bool jump, bool restrictForward = false, bool sprint = false, bool dodge = false)
    {
        base.Move(move, crouch, jump, restrictForward, sprint, dodge);
    }

    public override void ChangeMovementType(MovementType type)
    {
        base.ChangeMovementType(type);
    }
}
