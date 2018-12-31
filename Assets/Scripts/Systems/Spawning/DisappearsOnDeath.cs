using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearsOnDeath : OnDeathBase {
    

	// Use this for initialization
	void Start () {
		
	}

    public override void OnDeath()
    {
        GetComponent<Collider>().isTrigger = true;
        base.Flourish();
        StartCoroutine("Die");
    }

    public IEnumerator Die()
    {
        yield return new WaitForSeconds(10f);
        base.OnDeath();
    }
}
