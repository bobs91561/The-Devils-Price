using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CrossHair : MonoBehaviour {

    public static CrossHair instance;

    public Transform ObjectToProjectFrom;
    public float MaxDistance;

    private LayerMask _mask;
    private Image image;
    private Color baseColor;
    private GameObject Player;

    private bool isInitialized;

	// Use this for initialization
	void Start () {
        _mask = LayerMask.GetMask("Enemy");
        image = GetComponent<Image>();
        baseColor = image.color;
        isInitialized = false;
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EventManager.SubscribeToPlayer += UpdatePlayer;
	}

    private void Initialize()
    {
        if (Player == null)
        {
            Player = GameManager.Player;
        }
        if (ObjectToProjectFrom == null)
        {
            ObjectToProjectFrom = GameManager.Player.GetComponent<SkillSet>().castingObject.transform;
        }
        isInitialized = true;
    }

    
    public void UpdateCrossHair (Vector3 fwd) {
        if (!isInitialized) Initialize();
        if (!Player) Initialize();
        Vector3 vec = Player.transform.position;
        vec.y = ObjectToProjectFrom.position.y;
        Debug.DrawRay(vec, fwd*MaxDistance, Color.green);
        RaycastHit hit;
        bool hitSomething = Physics.SphereCast(vec, 0.15f, fwd, out hit, MaxDistance, _mask);
        var hm = hitSomething ? hit.collider.gameObject.GetComponent<HealthManager>() : null;
        if (hitSomething && hm && hm.isAlive)
        {
            
            image.color = Color.red;
        }
        else

            image.color = baseColor;
    }

    public static void FindInstance()
    {
        CrossHair ch = GameObject.Find("CrossHair").GetComponent<CrossHair>();
        if (!ch) Debug.Log("Can't find anything");
        else instance = ch;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (instance == null) instance = this;
        Player = GameManager.Player;
        Initialize();
    }

    public void UpdatePlayer()
    {
        Player = GameManager.Player;
        ObjectToProjectFrom = GameManager.Player.GetComponent<SkillSet>().castingObject.transform;
    }
    
}
