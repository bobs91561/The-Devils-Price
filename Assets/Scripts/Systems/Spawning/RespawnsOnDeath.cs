using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnsOnDeath : OnDeathBase {

    private Vector3 _mRespawnPosition;
    private Quaternion _mRespawnRotation;
    public Zone zone;
    public float TimeStamp;

	// Use this for initialization
	void Start () {
        enabled = false;
        _mRespawnPosition = transform.position;
        _mRespawnRotation = transform.rotation;
        
        if (!zone) zone = GetComponentInParent<Zone>();
	}

    public override void OnDeath()
    {
        GetComponent<Collider>().isTrigger = true;
        StartCoroutine("Die");
    }

    public void Respawn()
    {
        Debug.Log("Calling respawn");
        transform.position = _mRespawnPosition;
        transform.rotation = _mRespawnRotation;
        GetComponent<Collider>().isTrigger = false;
        gameObject.SetActive(true);
        SendMessage("Life");
    }

    public IEnumerator Die()
    {
        yield return new WaitForSeconds(10f);
        base.OnDeath();
        SpawnManager.instance.NewRequest(this);
        TimeStamp = Time.time;
    }
    private void OnDisable()
    {
        if(SpawnManager.instance)
            SpawnManager.instance.RemoveRequest(this);
    }
}
