using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the continuous difficulty determination
/// </summary>
public static class DifficultyManager
{        
    private static float DetermineSpawnFactor(DifficultySetting setting)
    {
        return 0f;
    }

    private static float DetermineDensityFactor(DifficultySetting setting)
    {
        return 0f;
    }

    public static Vector3 DetermineZoneFactors(DifficultySetting setting)
    {
        Vector3 factors = new Vector3();

        factors.x = DetermineSpawnFactor(setting);
        factors.y = DetermineDensityFactor(setting);

        return factors;
    }

}

public enum DifficultySetting
{
    EASY, NORMAL, HARD, EPIC
}