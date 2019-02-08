using com.ootii.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Update CameraController anchor
        GameObject.Find("Camera_Controller").GetComponent<CameraController>().Anchor = transform;
        //Update CrossHair instance
        if (!CrossHair.instance) CrossHair.instance = GameObject.Find("CrossHair").GetComponent<CrossHair>();

        ExperienceHolder.Player = AIActionDecider.Player = AIAttackController.Player = GameManager.Player = gameObject;
        AIAttackController.PlayerCenter = GetComponent<SkillSet>().characterCenter;
    }
}
