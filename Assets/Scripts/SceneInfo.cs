using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{

    public List<GameObject> ObjectsToActivate;
    public GameObject EntryPoint;
    public Vector3 EntryOffset;

    private void Start()
    {
        EntryOffset = EntryPoint.GetComponent<RespawnPoint>().offset;
    }
}
