using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BargainContainer : ScriptableObject {

    public static BargainContainer instance;

    [SerializeField]private List<GameObject> PrincesAccepted;

    private void Awake()
    {
        instance = this;
        PrincesAccepted = new List<GameObject>();
    }

    public void AddPrince(GameObject g)
    {
        PrincesAccepted.Add(g);
    }

    public List<GameObject> GetPrinces()
    {
        return PrincesAccepted;
    }
}
