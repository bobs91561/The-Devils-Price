using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomManager {
    /// <summary>
    /// This holds the keycode and axes names of user controls.
    /// This script will modify based on platform.
    /// PlayerController will pull from this script to get relevant keys.
    /// 
    /// Default Key Mappings (for Xbox One):
    ///     A -> Use Object/Conversation
    ///     B -> Draw Weapons
    ///     X -> Dodge
    ///     Y -> Jump
    ///     RB -> Light Attack
    ///     RT -> Heavy Attack
    ///     LT -> Light Attack Left
    ///     RT + LT -> Block
    ///     LB + RB -> Charged Attack
    ///     LB + (A, B, X, Y) -> Magic Attack
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        public InputMap WindowsOS;
        public InputMap MacOS;
        public InputMap Keyboard;

        public bool UseKeyboard;
        
        [HideInInspector]public string RightTrigger;
        [HideInInspector] public string LeftTrigger;

        [HideInInspector] public KeyCode LeftBumper;
        [HideInInspector] public KeyCode RightBumper;

        [HideInInspector] [Tooltip("For using objects")] public KeyCode KeyOne;
        [HideInInspector] [Tooltip("For drawing weapons")] public KeyCode KeyTwo;
        [HideInInspector] [Tooltip("For dodging")] public KeyCode KeyThree;
        [HideInInspector] [Tooltip("For jumping")] public KeyCode KeyFour;

        [HideInInspector] public KeyCode MenuKey;
        [HideInInspector] public KeyCode MapKey;

        [HideInInspector] public KeyCode SprintKey;
        [HideInInspector] public KeyCode JumpKey;
        [HideInInspector] public KeyCode DodgeKey;
        [HideInInspector] public KeyCode UseKey;
        [HideInInspector] public KeyCode CancelKey;

        // Start is called before the first frame update
        void Start()
        {
            //Singleton class
            instance = this;

            //Determine the platform and inputs
            DeterminePlatform();
        }

        private void DeterminePlatform()
        {
            if (Input.GetJoystickNames().Length <= 0) UseKeyboard = true;

            if (UseKeyboard)
            {
                SetUpInput(Keyboard);
            }
            else if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                //Set Keys and Axes to Xbox controller for Mac
                //Use MacOS InputMap
                SetUpInput(MacOS);
            }
            else if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                //Use Windows InputMap
                SetUpInput(WindowsOS);
            }
        }

        private void SetUpInput(InputMap map)
        {
            RightTrigger = map.RightTrigger;
            LeftTrigger = map.LeftTrigger;
            RightBumper = map.RightBumper;
            LeftBumper = map.LeftBumper;
            KeyOne = map.KeyOne;
            KeyTwo = map.KeyTwo;
            KeyThree = map.KeyThree;
            KeyFour = map.KeyFour;
            MenuKey = map.MenuKey;
            MapKey = map.MapKey;
            JumpKey = map.JumpKey;
            SprintKey = map.SprintKey;
            DodgeKey = map.DodgeKey;
            UseKey = map.UseKey;
            CancelKey = map.CancelKey;
        }

        /// <summary>
        /// Populates input-dependent scripts
        /// </summary>
        private void PopulateDependencies()
        {
            //Modify Player Selector/Proximity Selector Use Key
        }

        public static void SetSelectorUseKey(GameObject g)
        {
            var selector = g.GetComponentInChildren<Selector>();
            if (selector)
            {
                selector.useKey = instance.UseKey;
            }
            var prox = g.GetComponentInChildren<ProximitySelector>();
            if (prox)
            {
                prox.useKey = instance.UseKey;
            }
        }

        public static void SetDialogueCancelKeys(GameObject g)
        {
            var dialogue = g.GetComponent<DialogueSystemController>();
            if (dialogue)
            {
                dialogue.displaySettings.inputSettings.cancel.key = instance.CancelKey;
                dialogue.displaySettings.inputSettings.cancelConversation.key = instance.CancelKey;
            }
        }
        
        public static KeyCode[] GetBumpers()
        {
            return new KeyCode[2] { instance.LeftBumper, instance.RightBumper };
        }

        public static string[] GetTriggerAxes()
        {
            return new string[2] { instance.LeftTrigger, instance.RightTrigger };
        }

        public static KeyCode[] GetOtherKeys()
        {
            return new KeyCode[4] { instance.KeyOne, instance.KeyTwo, instance.KeyThree, instance.KeyFour };
        }

    }
}
