using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using UnityEngine.SceneManagement;
using com.ootii.Cameras;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static Inventory inventory;
    public static Journal journal;
    public static GameObject Player;

    public static List<Zone> ActiveZones;
    public static Zone CurrentZone;

    public List<GameObject> ObjectsToActivate;
    public ExecuteOnComplete exec;

    public GameObject PlayerPrefab;


    private void Start()
    {
        if (GameObject.Find("Player") == null) {
            GameObject g = Instantiate(PlayerPrefab);
            FindObjectOfType<CameraController>().Anchor = g.transform;
            g.GetComponent<HealthManager>().healthSlider = GameObject.Find("Health").GetComponent<Slider>();
        }
        if (ActiveZones == null) ActiveZones = new List<Zone>();
        EventManager.DeathAction += ReloadScene;
        exec.AddObjects(ObjectsToActivate);
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
    

}
