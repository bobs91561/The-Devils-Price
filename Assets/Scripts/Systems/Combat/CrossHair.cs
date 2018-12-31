using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour {

    public static CrossHair instance;

    public GameObject ObjectToProjectFrom;
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
	}

    private void Initialize()
    {
        if (Player == null)
        {
            Player = GameManager.Player;
        }
        if (ObjectToProjectFrom == null)
        {
            ObjectToProjectFrom = GameManager.Player.GetComponent<SkillSet>().castingObject;
        }
        isInitialized = true;
    }

    
    public void UpdateCrossHair (Vector3 fwd) {
        if (!isInitialized) Initialize();
        Debug.DrawRay(Player.transform.position, fwd*MaxDistance, Color.green);
        RaycastHit hit;
        bool hitSomething = Physics.SphereCast(Player.transform.position, 0.15f, fwd, out hit, MaxDistance, _mask);
        var hm = hitSomething ? hit.collider.gameObject.GetComponent<HealthManager>() : null;
        if (hitSomething && hm && hm.isAlive)
        {
            
            image.color = Color.red;
        }
        else

            image.color = baseColor;
    }
}
