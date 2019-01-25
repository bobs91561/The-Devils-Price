using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomManager {
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

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
        public string RightTrigger;
        public string LeftTrigger;

        public KeyCode LeftBumper;
        public KeyCode RightBumper;

        [Tooltip("For using objects")] public KeyCode KeyOne;
        [Tooltip("For drawing weapons")] public KeyCode KeyTwo;
        [Tooltip("For dodging")] public KeyCode KeyThree;
        [Tooltip("For jumping")] public KeyCode KeyFour;

        public KeyCode MenuKey;
        public KeyCode MapKey;

        public KeyCode SprintKey;
        public KeyCode JumpKey;

        // Start is called before the first frame update
        void Start()
        {
            //Singleton class
            instance = this;

            //Determine the platform and inputs
            //Populate all dependent scripts
        }

        /// <summary>
        /// Populates input-dependent scripts
        /// </summary>
        private void PopulateDependencies()
        {

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
