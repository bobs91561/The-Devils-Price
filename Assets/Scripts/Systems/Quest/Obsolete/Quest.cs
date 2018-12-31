using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QuestSystem
{
    [CreateAssetMenu]
    public class Quest : ScriptableObject
    {
        public string Name;
        public string Description;

        public List<Objective> Objectives;
        public Objective currentObjective;
        private int curr = 0;
        public List<Reward> rewards;

        public void BeginQuest()
        {
            currentObjective = Objectives[0];
            currentObjective.SetActive();
        }

        public void AdvanceQuest()
        {
            if (currentObjective.completed && curr == Objectives.Capacity - 1)
                CompleteQuest();
            else if (currentObjective.completed)
            {
                curr++;
                currentObjective = Objectives[curr];
                currentObjective.SetActive();
            }

        }

        public void CompleteQuest()
        {
            foreach (Reward r in rewards)
                r.GetReward();
        }

        public void FailQuest()
        {

        }
    }
}
