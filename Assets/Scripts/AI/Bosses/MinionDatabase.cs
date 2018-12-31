using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MinionDatabase")]
public class MinionDatabase : ScriptableObject
{
    public static MinionDatabase instance;

    [Header("FIRE")]
    public List<GameObject> Minions;

    [Header("SHADOW")]
    public List<GameObject> ShadowMinions;

    public GameObject DetermineMinion()
    {
        return null;
    }
}
