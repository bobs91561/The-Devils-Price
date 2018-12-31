using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem {
    [CreateAssetMenu]
    public class Objective : ScriptableObject {

        public GameObject Target;
        public int collectionNumber;

        public string Task;
        public TaskType Type;

        public bool completed = false;
        public bool active = false;

        private List<Reward> rewards;

        public void SetActive()
        {
            active = true;
        }

        public void CompleteObjective()
        {
            completed = true;
        }

        private void CheckObjective()
        {
            switch (Type)
            {
                case TaskType.Kill:
                    CheckKill();
                    break;
                case TaskType.Follow:
                    CheckFollow();
                    break;
                case TaskType.Find:
                    CheckFind();
                    break;
                case TaskType.Collect:
                    CheckCollect();
                    break;
            }
        }
        /// <summary>
        /// Check to see if the Target is dead
        /// </summary>
        private void CheckKill()
        {

        }
        /// <summary>
        /// Check to see if the player is within follow distance
        /// </summary>
        private void CheckFollow()
        {

        }
        /// <summary>
        /// Check to see if the player has found the Target
        /// </summary>
        private void CheckFind()
        {

        }
        /// <summary>
        /// Check to see if the player has the required amount of the Target
        /// </summary>
        private void CheckCollect()
        {

        }
    }

    public enum TaskType
    {
        Collect, Kill, Follow, Find
    }
}