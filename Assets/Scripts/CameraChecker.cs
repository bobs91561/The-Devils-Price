using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Cameras;

public class CameraChecker : MonoBehaviour
{
    
    // Update is called once per frame
    void LateUpdate()
    {
        if (!GetComponent<CameraController>().Anchor)
        {
            GetComponent<CameraController>().Anchor = GameObject.Find("Player").transform;
        }
        else
            enabled = false;
    }
}
