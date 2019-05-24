using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDelay : MonoBehaviour
{
    public SphereCollider DelayedCollider;
    public float DelayTime;
    private float _start_time;

    void Start()
    {
        DelayedCollider.enabled = false;
        _start_time = Time.time;
    }

    void Update()
    {
        if (Time.time > _start_time + DelayTime)
            DelayedCollider.enabled = true;
    }
}
