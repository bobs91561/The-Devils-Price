using UnityEngine;
using System.Collections;

namespace Devdog.InventoryPro.UnityStandardAssets
{
	//[RequireComponent(typeof(Rigidbody))]
	//[RequireComponent(typeof(CapsuleCollider))]
	// [RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] public float m_MovingTurnSpeed = 360;
		[SerializeField] public float m_StationaryTurnSpeed = 180;
		[SerializeField] public float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] public float m_GravityMultiplier = 2f;
		[SerializeField] public float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] public float m_MoveSpeedMultiplier = 1f;
		[SerializeField] public float m_AnimSpeedMultiplier = 1f;
		[SerializeField] public float m_GroundCheckDistance = 0.1f;
        [SerializeField] public float m_DodgeSpeedMultiplier = 2f;

        public AudioClip footstep;

        Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;


        private int _dodgeID = Animator.StringToHash("Dodge");
        private int _sprintID = Animator.StringToHash("SprintKey");
        private int _jumpID = Animator.StringToHash("JumpTrigger");
        private int _forwardID = Animator.StringToHash("Forward");

        //public bool m_IsReacting;
        public bool IsRotating;
        [SerializeField]private bool m_IsJumping;
        [SerializeField] private bool m_ReadyToJump;

        public bool m_IsDodging;
        private bool m_ReadyToDodge;

	    public LayerMask raycastLayerMask;


        void Start()
		{
			m_Animator = GetComponent<Animator>();
            if (!m_Animator) m_Animator = GetComponentInChildren<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();

            if (!m_Rigidbody)
            {
                m_Rigidbody = GetComponentInParent<Rigidbody>();
                m_Capsule = GetComponentInParent<CapsuleCollider>();
            }

            m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

            

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}


	    void OnDisable()
	    {
	        m_ForwardAmount = 0;
            if(m_Animator)
                m_Animator.SetFloat(_forwardID, 0f);
	    }
        

		public void Move(Vector3 move, bool crouch, bool jump, bool restrictForward = false, bool sprint = false, bool dodge = false)
		{
#if UMA
			if(m_Animator == null)
			{
				m_Animator = GetComponent<Animator>();
			}
#endif

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;
			ApplyExtraTurnRotation();
            m_ForwardAmount = restrictForward ? 0f : move.z;

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}
            
            if (jump && !m_IsJumping && m_IsGrounded)
            {
                m_Animator.SetTrigger(_jumpID);
                m_IsJumping = true;
                m_ReadyToJump = false;
            }

            if(dodge && !m_IsDodging)
            {
                var id = DetermineDodgeDirection(move);
                m_Animator.SetTrigger(id);
                m_IsDodging = true;
            }

            ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

            // send input and other state parameters to the animator

            m_Animator.SetBool(_sprintID, sprint);
            UpdateAnimator(move);
		}


		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, raycastLayerMask))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, raycastLayerMask))
				{
					m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat(_forwardID, Mathf.Abs(m_ForwardAmount), 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);
			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded)
			{
				m_Animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}

		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.fixedDeltaTime, 0);
		}


		public void OnAnimatorMove()
		{
            Vector3 v;
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.

            if (m_IsJumping && m_ReadyToJump)
            {
                // jump!
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_Animator.applyRootMotion = false;
                m_GroundCheckDistance = 0.1f;
                m_IsJumping = false;
                m_ReadyToJump = false;
            }
            else if(m_IsDodging && Time.deltaTime > 0)
            {
                v = (m_Animator.deltaPosition * m_DodgeSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
            }
            else if (m_IsGrounded && Time.deltaTime > 0)
			{
				v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;

            }

        }
        

		void CheckGroundStatus()
		{
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance), Color.blue);
#endif
            RaycastHit hitInfo;
            
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance, raycastLayerMask))
			{
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                m_Animator.applyRootMotion = true;

                return;
            }


            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            m_Animator.applyRootMotion = false;
        }

        public void Jump()
        {
            m_ReadyToJump = true;
        }

        private int DetermineDodgeDirection(Vector3 move)
        {
            //if the x-z vector components are both 0, dodge backwards

            //if z is 0 and x is positive, dodge right

            //if z is 0 and x is negative, dodge left

            //otherwise, dodge-roll
            return _dodgeID;
        }

        public void EndDodge()
        {
            m_IsDodging = false;
        }
    }
}
