using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestSystem;

public class Journal : ScriptableObject {

    public List<Quest> quests;

	// Use this for initialization
	void Awake () {
        quests = new List<Quest>();
	}
	

    public void AcquireQuest(Quest q)
    {
        if (q && !quests.Contains(q))
        {
            quests.Add(q);
            q.BeginQuest();
        }
    }
}
