using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/ExecuteOnComplete")]
public class ExecuteOnComplete : ScriptableObject {

    public static GameObject Player;

    public List<GameObject> ObjectsToActivate;
    private List<string> ObjectNames;
    

    public void Execute()
    {

    }

    public void AddObjects(List<GameObject> objs)
    {
        ObjectsToActivate = objs;
        ObjectNames = new List<string>();
        foreach (GameObject g in ObjectsToActivate)
        {
            ObjectNames.Add(g.name);
        }
    }

    /// <summary>
    /// Triggers OnAggression of a FriendlyConditional script
    /// </summary>
    /// <param name="g"></param>
    protected void TriggerCombat(GameObject g)
    {
        //Trigger the combat of an AI
        FriendlyConditional friendly = g.GetComponent<FriendlyConditional>();
        if (!friendly) return;
        friendly.OnAggression();
    }

    public void TriggerCombat(string name)
    {
        GameObject g = GameObject.Find(name);
        TriggerCombat(g);
    }

    /// <summary>
    /// Add experience to the player
    /// </summary>
    /// <param name="exp"></param>
    public void AddExperience(float exp)
    {
        //Add experience to player
        ExperienceHolder.AddExperience(exp);
    }

    public void AddDemonicPower(float power)
    {
        //Add demonic power to player
        
    }

    public void AddReputation(float reputation)
    {
        //Add reputation to player
    }

    public void TakeBargain()
    {
        //If player takes a bargain-based quest, add the bargainer to the final list
    }
    /// <summary>
    /// Activates object in scene with name
    /// </summary>
    /// <param name="name"></param>
    public void ActivateObject(string name)
    {
        if (!ObjectNames.Contains(name)) return;

        GameObject g = ObjectsToActivate[ObjectNames.IndexOf(name)];
        if (!g) return;
        g.SetActive(true);
    }
}
