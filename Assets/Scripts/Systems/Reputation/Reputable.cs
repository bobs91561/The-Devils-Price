using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
/// <summary>
/// Handles the "memory" of demonic forces.
/// When the player dies, their reputation among the underworld is decreased.
/// The delta amount is determined by the player's level vs the level of what killed them
/// When the player defeats an enemy, their reputation is increased.
/// Only enemies with a greater reputation increase the player's rep.
/// </summary>
public class Reputable : MonoBehaviour {

    public float Reputation;

	// Use this for initialization
	void Start () {
	}

    public void ModifyReputation(Reputable r, bool death)
    {
        float f = r.Reputation;
        //Did the reputable die?
        if (death)
        {
            //Yes, decrease reputation
            if (f < Reputation) DecreaseReputation((f - Reputation)/2);
            else DecreaseReputation((Reputation - f) / 3);
        }
        else
        {
            //No, check the other reputable's reputation
            //if larger, add to reputation
            if (f > Reputation)
                IncreaseReputation((f - Reputation) / 2);
        }
    }

    public void DecreaseReputation(float amount)
    {
        Reputation -= amount;
    }

    public void IncreaseReputation(float amount)
    {
        Reputation += amount;
    }

    public void OnDeathParam(GameObject g)
    {
        Reputable r = g.GetComponent<Reputable>();
        if (!r) DecreaseReputation(0.5f);
        else
        {
            ModifyReputation(r, true);
            r.ModifyReputation(this, false);
        }
    }
}
