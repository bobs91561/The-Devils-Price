using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoningData : MonoBehaviour {

    public DifficultySetting ZoneSetting;

    public float EnemyDensity;
    public float SpawnRate = 1f;
    
	// Use this for initialization
	void Start () {
		
	}

    private void ModifySettings()
    {
        Vector3 factors = DifficultyManager.DetermineZoneFactors(ZoneSetting);
        SpawnRate = factors.x;
        EnemyDensity = factors.y;
    }
}

