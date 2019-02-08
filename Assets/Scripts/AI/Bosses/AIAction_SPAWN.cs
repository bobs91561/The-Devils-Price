using System;
using UnityEngine;

/// <summary>
/// This class is used to determine if the spawn point chosen is valid, and spawns enemies of the appropriate type
/// </summary>
public class AIAction_SPAWN : AIAction
{
    public GameObject Spawn;
    public GameObject EnemyType;
    private GameObject SpawnPoint;
    
	void Start()
    {

	}

    public override bool Tick()
    {
       // SpawnPoint = Spawn.range();
        if (!ActionFeasible()) return false;
        return true;
    }

    public override bool ActionFeasible()
    {
        //if (SpawnPoint == validMeshPosition) return true;
        return false;
    }
    
}
