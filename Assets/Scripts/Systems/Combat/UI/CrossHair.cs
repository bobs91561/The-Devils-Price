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
    private SkillSet _skillSet;

    private GameObject SimpleObject;
    [SerializeField]private GameObject simplePrefab;

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
            _skillSet = Player.GetComponent<SkillSet>();
        }
        if (ObjectToProjectFrom == null && Player.GetComponent<SkillSet>())
        {
            ObjectToProjectFrom = GameManager.Player.GetComponent<SkillSet>().characterCenter.transform;
        }
        isInitialized = true;
        SimpleObject = Instantiate(simplePrefab);
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
            _skillSet.TargetThis(hm.gameObject.GetComponent<SkillSet>().characterCenter);
        }
        else
        {
            image.color = baseColor;
            Ray ray = new Ray(vec, fwd * MaxDistance);
            var pos = ray.origin + (ray.direction * MaxDistance);
            SimpleObject.transform.position = pos;
            _skillSet.TargetThis(SimpleObject);
        }
    }

    public static void FindInstance()
    {
        GameObject go = GameObject.Find("CrossHair");
        if (!go)
        {
            return;
        }
        CrossHair ch = go.GetComponent<CrossHair>();
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
        ObjectToProjectFrom = GameManager.Player.GetComponent<SkillSet>().characterCenter.transform;
    }

    public void Deactivate()
    {
        GetComponent<Image>().enabled = false;
    }

    public void Activate()
    {
        GetComponent<Image>().enabled = true;
    }
    
}
