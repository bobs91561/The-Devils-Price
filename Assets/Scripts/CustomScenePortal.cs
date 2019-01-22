using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomScenePortal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.HandleFadeManually = true;
        GameManager.SetPlayerInput(false);
    }
}
