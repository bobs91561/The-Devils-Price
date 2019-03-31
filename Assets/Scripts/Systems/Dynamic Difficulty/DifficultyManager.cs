using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the continuous difficulty determination
/// </summary>
public static class DifficultyManager
{
    private static float m_MostRecentCombatTime;
    private static float m_MostRecentCombatLength;
    private static float m_MostRecentPlayerDeath = 0f;
    private static float m_MostRecentDeathDifference = 0f;

    private static DifficultySetting m_DifficultySettingOfVictor;
    private static int m_RankOfVictor;

    private static float m_PlayerDeathAverageTimes;
    private static int m_PlayerDeaths = 0;

    private static float m_PlayerCombatAverageTimes;
    private static int m_CombatInstances = 0;

    private static float m_AverageDifficulty;

    private static float m_AcceptedRangeBelowAverage = 0.8f;

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

    public static void EnterPlayerCombat()
    {
        // Track the time of the combat start
        m_MostRecentCombatTime = Time.time;
    }
    
    public static void ExitCombat()
    {
        var end = Time.time;
        m_MostRecentCombatLength = end - m_MostRecentCombatTime;
        m_CombatInstances++;
        m_PlayerCombatAverageTimes += m_MostRecentCombatTime / m_CombatInstances;
    }

    /// <summary>
    /// Called when the player dies in combat. Modifies necessary tracking metrics.
    /// </summary>
    public static void UpdatePlayerDeath()
    {
        // Track the time of this death
        m_MostRecentDeathDifference = m_MostRecentPlayerDeath - Time.time; ;

        m_MostRecentPlayerDeath = Time.time;
        m_PlayerDeaths++;

        m_PlayerDeathAverageTimes += m_MostRecentDeathDifference / m_PlayerDeaths;

        // Grab the victor's difficulty setting
        var victor = GameManager.Player.GetComponent<HealthManager>().hitBy;
        m_RankOfVictor = victor.GetComponent<Reputable>().CurrentRank;
        m_DifficultySettingOfVictor = victor.GetComponentInParent<ZoningData>().ZoneSetting;
    }

    /// <summary>
    /// Decides whether the player's death deserves a response
    /// </summary>
    public static void ConsiderResponse()
    {
        // Compare the most recent death times
        //      if the times are acceptable, do not respond
        //      if the times are lengthy, consider a response
        if (m_MostRecentDeathDifference >= m_PlayerDeathAverageTimes * m_AcceptedRangeBelowAverage) return;

        // Find the amount of time the player spent in combat
        if (m_MostRecentCombatTime >= m_PlayerCombatAverageTimes * m_AcceptedRangeBelowAverage) return;

        // Compare the combat time to the expected time for the enemy's difficulty setting and rank
        //      if the times are acceptable, do not respond
        //      if the times are lengthy, consider a response
        if (((int)m_DifficultySettingOfVictor >= m_AverageDifficulty || m_RankOfVictor > GameManager.Player.GetComponent<Reputable>().CurrentRank) 
                && m_MostRecentCombatTime < m_PlayerCombatAverageTimes * m_AcceptedRangeBelowAverage) return;

        RespondToPlayerDeath();
    }

    /// <summary>
    /// This method responds to the player's death in combat
    /// </summary>
    public static void RespondToPlayerDeath()
    {
        // Perform modifications using EventManager
        EventManager.TriggerDDA();
    }

}

/// <summary>
/// Keeps track of combat entries and exits and passes these values to the DifficultyManager
/// </summary>
public static class CombatManager
{
    private static List<GameObject> m_Combatants = new List<GameObject>();

    public static void EnterCombat(GameObject enemy)
    {
        // Add the enemy to the list of active combatants
        if (m_Combatants.Contains(enemy)) return;
        m_Combatants.Add(enemy);
        //If the list of combatants is empty, tell the difficulty manager to track the entrance
        if (m_Combatants.Count <= 1) DifficultyManager.EnterPlayerCombat();
    }

    public static void ExitCombat(GameObject enemy)
    {
        // Remove the enemy from the list of enemies
        if (!m_Combatants.Contains(enemy)) return;
        m_Combatants.Remove(enemy);
        // if the list of combatants is empty, tell the difficulty manager to track time of fight
        if (m_Combatants.Count <= 0) DifficultyManager.ExitCombat();
    }
}

public enum DifficultySetting
{
    EASY = 0, NORMAL = 1, HARD = 2, EPIC = 3
}