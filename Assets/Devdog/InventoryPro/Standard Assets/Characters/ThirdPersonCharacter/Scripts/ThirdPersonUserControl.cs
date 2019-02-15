using System;
using System.Collections.Generic;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;
using CustomManager;
using System.Collections;

namespace Devdog.InventoryPro.UnityStandardAssets
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour//, IPlayerInputCallbacks
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        public Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;

        private Transform m_Parent;
        private PlayerController m_PlayerControl;

        private KeyCode m_SprintKey;
        private KeyCode m_JumpKey;
        private KeyCode m_DodgeKey;

        public float walkSpeedMultilpier = 0.5f;

        private bool MoveToCamera;

        public bool isSimplePlayer;

        protected void Start()
        {


            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            m_PlayerControl = GetComponent<PlayerController>();
            /*
            var player = PlayerManager.instance.currentPlayer;
            if (player != null)
            {
                player.inventoryPlayer.stats.OnStatValueChanged += CharacterCollectionOnOnStatChanged;
                var stat = player.inventoryPlayer.stats.Get("Default", "Run speed");
                if (stat != null)
                {
                    CharacterCollectionOnOnStatChanged(stat);
                }
            }*/

            StartCoroutine(SetUpInput());
        }

        private IEnumerator SetUpInput()
        {
            yield return new WaitWhile(() => !CustomManager.InputManager.instance);
            m_SprintKey = CustomManager.InputManager.instance.SprintKey;
            m_JumpKey = CustomManager.InputManager.instance.JumpKey;
            m_DodgeKey = CustomManager.InputManager.instance.DodgeKey;
        }

        private void CharacterCollectionOnOnStatChanged(IStat stat)
        {
            if (stat.definition.statName == "Run speed")
            {
                walkSpeedMultilpier = stat.currentValue / 100f;
            }
        }

        public void SetCamera()
        {
            var cam = Camera.main;
            if (m_Cam)
                m_Cam = cam.transform;
            else
            {
                var controller = FindObjectOfType<CameraChecker>().MainCam.transform;
            }
        }

        public void SetCamera(Camera main)
        {
            m_Cam = main.transform;
        }

        public void SetInputActive(bool active)
        {
            this.enabled = active;
            if (!m_Character) m_Character = GetComponent<ThirdPersonCharacter>();
            m_Character.enabled = active;
            //            if (active == false)
            //            {
            //                GetComponent<Rigidbody>().velocity = Vector3.zero;
            //            }
        }

        public void MoveTowardCameraForward(bool move)
        {
            MoveToCamera = move;
        }

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (m_Cam == null) SetCamera();
            if (MoveToCamera && m_Cam != null)
            {
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                //m_Move = m_CamForward;

                //m_Character.Move(m_Move, false, false, true);
                transform.forward = Vector3.Lerp(transform.forward, m_CamForward, 12f * Time.fixedDeltaTime);
                if (!CrossHair.instance && !isSimplePlayer) CrossHair.FindInstance();
                if (CrossHair.instance && !isSimplePlayer) CrossHair.instance.UpdateCrossHair(m_CamForward);
                return;
            }
            if (!isSimplePlayer && GetComponent<SkillSet>().CheckAttack()) return;

            // read inputs
            float h = Input.GetAxis("Horizontal"); // CrossPlatformInputManager
            float v = Input.GetAxis("Vertical"); // CrossPlatformInputManager
            //bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                if (!CrossHair.instance && !isSimplePlayer) CrossHair.FindInstance();
                if (CrossHair.instance && !isSimplePlayer) CrossHair.instance.UpdateCrossHair(m_CamForward);
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
            bool sprint = false;
            // walk speed multiplier
            if (Input.GetKey(m_SprintKey))
            {
                m_Move *= (walkSpeedMultilpier * 5);
                sprint = true;
            }
            else
            {
                m_Move *= walkSpeedMultilpier;
            }
            bool jump = false;
            if ((isSimplePlayer || m_PlayerControl.Mode != PlayerMode.COMBAT) && Input.GetKeyDown(m_JumpKey))
                jump = true;

            bool dodge = false;
            if (Input.GetKeyDown(m_DodgeKey))
            {
                dodge = true;
            }

            // pass all parameters to the character control script
            m_Character.Move(m_Move, false, jump, sprint: sprint, dodge: dodge);
        }

        void OnDisable()
        {
            m_Character.Move(Vector3.zero, false, false);
        }

        public void OnDeath()
        {
            enabled = false;
        }

        public void OnRespawn()
        {
            enabled = true;
        }

    }
}