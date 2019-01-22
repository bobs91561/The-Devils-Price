using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OnPlayerDeath : MonoBehaviour
{
    private Vector3 _mResetPosition;
    private Quaternion _mResetRotation;
    private NavMeshAgent _mAgent;

    private HealthManager m_HealthManager;

    private bool removed;

    // Start is called before the first frame update
    void Start()
    {
        _mResetPosition = transform.position;
        _mResetRotation = transform.rotation;
        _mAgent = GetComponent<NavMeshAgent>();
        m_HealthManager = GetComponent<HealthManager>();
        EventManager.DeathAction += ResetOnDeath;
        removed = false;
    }

    public void ResetOnDeath()
    {
        //Reset position
        _mAgent.Warp(_mResetPosition);
        transform.rotation = _mResetRotation;

        //Reset health
        m_HealthManager.ResetHealth();
    }

    public void OnDisable()
    {
        if (removed) return;
        EventManager.DeathAction -= ResetOnDeath;
        removed = true;
    }

    public void OnDestroy()
    {
        if (removed) return;
        EventManager.DeathAction -= ResetOnDeath;
        removed = true;
    }
}
