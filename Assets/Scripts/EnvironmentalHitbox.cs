using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalHitbox : MonoBehaviour {

    public float DamagePerSecond;

	// Use this for initialization
	void Start () {
		
	}

    internal void OnTriggerStay(Collider other)
    {
        HealthManager hm = other.gameObject.GetComponent<HealthManager>();
        if (!hm) return;

        hm.hitBy = gameObject;
        hm.ModifyHealth(-DamagePerSecond * Time.deltaTime);

    }

    internal void OnCollisionStay(Collision collision)
    {
        HealthManager hm = collision.gameObject.GetComponent<HealthManager>();
        if (!hm) return;

        hm.hitBy = gameObject;
        hm.ModifyHealth(-DamagePerSecond * Time.deltaTime);
    }

}
