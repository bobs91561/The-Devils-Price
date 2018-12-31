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
	
    public static void Death()
    {
        DeathAction();
    }

    public static void OffWorld()
    {
        FellOffWorld();
    }
}
