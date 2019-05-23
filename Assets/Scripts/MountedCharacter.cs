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

    public bool StartsRiding;

    public GameObject SecondaryObject;

    public float RidingBaseOffset;
    public float RidingHeight;
    public float RidingRadius;

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

        if (RidingHeight == 0) RidingHeight = RegularHeight;
        if (RidingRadius == 0) RidingRadius = RegularRadius;
        if (RidingBaseOffset == 0) RidingBaseOffset = RegularBaseOffset;

        if (SecondaryObject) m_SecondaryAnimator = SecondaryObject.GetComponent<Animator>();
        else
            m_SecondaryAnimator = GetComponentsInChildren<Animator>()[1];
    }

    public override void Move(Vector3 move, bool crouch, bool jump, bool restrictForward = false, bool sprint = false, bool dodge = false)
    {
        base.Move(move, crouch, jump, restrictForward, sprint, dodge);
        // If mounted, pass values to secondary animator
    }

    public override void ChangeMovementType(MovementType type)
    {
        base.ChangeMovementType(type);

        // Set layer weights
        
    }
}
