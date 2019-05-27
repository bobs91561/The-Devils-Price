using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDelay : MonoBehaviour
{
    public SphereCollider DelayedCollider;
    public float DelayTime; //Enable after _ seconds
    public float DisableTime; //Disable after _ seconds
    private float _start_time;

    void Start()
    {
        DelayedCollider.enabled = false;
        _start_time = Time.time;
    }

    void Update()
    {
        if (Time.time >= _start_time + DelayTime)
            DelayedCollider.enabled = true;
        if (DisableTime != 0 && Time.time > _start_time + DisableTime)
            DelayedCollider.enabled = false;
    }
}
