using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Class to hold all events and delegates used in the project
/// </summary>
public class EventManager {

    public delegate void VoidDelegate();
    public static event VoidDelegate DeathAction;

    public static event VoidDelegate Aggression;

    public static event VoidDelegate FellOffWorld;

    public static event VoidDelegate RespawnAction;

    public static event VoidDelegate SubscribeToPlayer;

    public static event VoidDelegate RankUp;
	
    public static void Death()
    {
        DeathAction();
    }

    public static void OffWorld()
    {
        FellOffWorld();
    }

    public static void Respawn()
    {
        RespawnAction();
    }

    public static void UpdatePlayer()
    {
        if(SubscribeToPlayer != null)
        SubscribeToPlayer();
    }

    public static void IncreaseRank()
    {
        if (RankUp != null)
            RankUp();
    }
}
