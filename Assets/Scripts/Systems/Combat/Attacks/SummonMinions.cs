using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Attacks/Boss/SummonMinions")]
public class SummonMinions : Attack
{
    public override void UseAttack()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Chooses which minion to generate
    /// </summary>
    private void DetermineMinion()
    {
        objectToGenerate = MinionDatabase.instance.DetermineMinion();
    }

    private void SpawnMultiple()
    {

    }
}

public enum SummonerType
{
    FIRE
}
