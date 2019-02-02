using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Handles the "memory" of demonic forces.
/// When the player dies, their reputation among the underworld is decreased.
/// The delta amount is determined by the player's level vs the level of what killed them
/// When the player defeats an enemy, their reputation is increased.
/// Only enemies with a greater reputation increase the player's rep.
/// </summary>
public class Reputable : MonoBehaviour {
    public static float BaseReputation = 20f;
    public float Reputation;
    public int CurrentRank;
    private float m_RankRequirement;

    private RankResponse m_RankResponse;
	// Use this for initialization
	void Start () {
        DetermineRankRequirement();
        m_RankResponse = GetComponent<RankResponse>();
	}

    private void DetermineRankRequirement()
    {
        //(Rank^2 - Rank - 1) * 75f
        m_RankRequirement = (CurrentRank * CurrentRank - CurrentRank - 1) * 75f;
        if (m_RankRequirement < 0f) m_RankRequirement = 0f;
    }

    private void CheckRank()
    {
        if (Reputation < m_RankRequirement)
            return;
        //Character has reached a new rank, increase the rank.
        UpdateRank();
    }

    private void UpdateRank()
    {
        CurrentRank++;
        DetermineRankRequirement();

        if (m_RankResponse) m_RankResponse.RankUp();
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
        CheckRank();
    }

    public void DecreaseReputation(float amount)
    {
        Reputation -= amount;
        if (Reputation < 0f)
            Reputation = BaseReputation;
    }

    public void IncreaseReputation(float amount)
    {
        Reputation += amount;
    }

    public void ForceRankUp()
    {
        Reputation = m_RankRequirement;
        CurrentRank++;
        DetermineRankRequirement();
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
