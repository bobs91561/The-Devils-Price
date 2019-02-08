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
        //Calculate how quickly enemies should respawn
        //Return a percentage of the distance that triggers spawning
        //  ie if spawn factor = 0.5 and distance for rsp is 50, then new distance = 25
        return 1f;
    }

    private static float DetermineDensityFactor(DifficultySetting setting)
    {
        //Calculate how dense the area should be based on the setting
        //Return an area percentage dictacting how much of free area should be populated with enemies
        //  ie if free area is 100 units and density factor is .2 then amt of free area to be populated with enemies is 20
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