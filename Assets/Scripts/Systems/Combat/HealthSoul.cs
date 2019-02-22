using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSoul : MonoBehaviour
{
    public float HealthSoulCapacity; //total amount of health soul can restore
    public float RestoreRate; //multiplier for rate health is restored at
    private float _maxCapacity;
    private PSMeshRendererUpdater _mesh;
    private Color _originalColor; //original color of the soul

    private void Start()
    {
        _maxCapacity = HealthSoulCapacity;
        _mesh = GetComponentInChildren<PSMeshRendererUpdater>();
        _originalColor = _mesh.Color;
    }

    private void OnTriggerStay(Collider collider)
    {
        float tick = Time.fixedDeltaTime * RestoreRate;
        if (tick > HealthSoulCapacity)
            return;

        GameObject player = collider.gameObject;
        if (player.tag == "Player")
        {
            HealthManager healthManager = player.GetComponent<HealthManager>();
            float missingHealth = (healthManager.maxHealth - healthManager.Health);

            if (missingHealth < tick)
            {
                //edge case to fill out remaining missing health that may be < tick
                if (missingHealth < HealthSoulCapacity)
                {
                    healthManager.ModifyHealth(missingHealth);
                    HealthSoulCapacity -= missingHealth;
                    _mesh.Color = Color.LerpUnclamped(Color.red, _originalColor, HealthSoulCapacity / _maxCapacity);
                }
                return;
            }

            healthManager.ModifyHealth(tick);
            HealthSoulCapacity -= tick;

            //update color of health soul (to red as capacity decreases)
            _mesh.Color = Color.LerpUnclamped(Color.red, _originalColor, HealthSoulCapacity/_maxCapacity);

        }
    }
}
