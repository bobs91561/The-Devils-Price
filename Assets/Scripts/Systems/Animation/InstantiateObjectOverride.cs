using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObjectOverride : TimelineOverrides
{

    public bool InstantiatePrefab;
    public GameObject Prefab;
    public Transform InstantiateWhereThisIs;

    public bool PrefabIsPlayer;
    public bool ReplaceObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OverrideTimeline()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        GameObject go = Instantiate(Prefab, InstantiateWhereThisIs.position, InstantiateWhereThisIs.rotation);
        if (ReplaceObject) Destroy(InstantiateWhereThisIs.gameObject);
        if (PrefabIsPlayer)
        {
            if (player) Destroy(player);
            GameManager.instance.SetUpPlayer();
        }
    }
}
