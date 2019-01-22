using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Cameras;

public class CameraChecker : MonoBehaviour
{
    public Camera MainCam;

    private void Start()
    {
        MainCam = GetComponentInChildren<Camera>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!GetComponent<CameraController>().Anchor && GameManager.Player)
        {
            GetComponent<CameraController>().Anchor = GameManager.Player.transform;
        }
        else
            enabled = false;
    }
}
