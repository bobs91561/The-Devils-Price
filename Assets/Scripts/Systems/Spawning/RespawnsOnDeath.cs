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
        Flourish();
        GetComponent<Collider>().isTrigger = true;
        if (!SpawnManager.instance)
        {
            Debug.Log("No SpawnManager present. Cannot respawn enemies.");
            return;
        }
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

        //destroy flourish mesh
        GameObject g = GetComponentInChildren<PSMeshRendererUpdater>().gameObject;
        if (g)
            Destroy(g);
    }

    public IEnumerator Die()
    {
        SpawnManager.instance.NewRequest(this);
        TimeStamp = Time.time;
        yield return new WaitForSeconds(5f);
        base.OnDeath();
    }
}
