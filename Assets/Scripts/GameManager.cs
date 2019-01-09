using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using UnityEngine.SceneManagement;
using com.ootii.Cameras;
using UnityEngine.UI;
using Devdog.InventoryPro.UnityStandardAssets;

public class GameManager : MonoBehaviour {
    public static GameObject Player;

    public static List<Zone> ActiveZones;
    public static Zone CurrentZone;

    private List<GameObject> ObjectsToActivate;
    public ExecuteOnComplete exec;

    public GameObject PlayerPrefab;

    public SceneInfo CurrentSceneData;

    public bool LoadingSave = false;


    private void Start()
    {
        /*if (GameObject.Find("Player") == null) {
            GameObject g = Instantiate(PlayerPrefab);
            FindObjectOfType<CameraController>().Anchor = g.transform;
            g.GetComponent<HealthManager>().healthSlider = GameObject.Find("Health").GetComponent<Slider>();
        }*/
        if (ActiveZones == null) ActiveZones = new List<Zone>();
        //EventManager.DeathAction += ReloadScene;
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this);
    }

    public void Loading()
    {
        LoadingSave = true;
    }

    private void PauseGame()
    {

    }


    private void ReloadScene()
    {
        //StartCoroutine("WaitToLoad");
    }

    private IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded");
        CurrentSceneData = GameObject.FindGameObjectWithTag("SceneInfo").GetComponent<SceneInfo>();
        ObjectsToActivate = CurrentSceneData.ObjectsToActivate;
        exec.AddObjects(ObjectsToActivate);
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if(players.Length > 1)
        {
            Destroy(players[1]);
            Player = GameObject.Find("Player");

            FindObjectOfType<CameraController>().Anchor = Player.transform;
            Player.GetComponent<HealthManager>().healthSlider = GameObject.Find("Health").GetComponent<Slider>();
        }
        else if (players.Length <= 0)
        {
            GameObject g = Instantiate(PlayerPrefab);
            g.name = "Player";
            Player = g;
            FindObjectOfType<CameraController>().Anchor = g.transform;
            g.GetComponent<HealthManager>().healthSlider = GameObject.Find("Health").GetComponent<Slider>();
            g.transform.position = CurrentSceneData.EntryPoint.transform.position + CurrentSceneData.EntryOffset;
        }
        if(Player)
            Player.GetComponent<ThirdPersonUserControl>().SetCamera();

    }
    

}
