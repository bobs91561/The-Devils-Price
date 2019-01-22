using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageForPreSetupObjects : MonoBehaviour {

    public float Damage;
    public GameObject _mParent;

    public float DestroyAfterSeconds = 10f;
    private float Timer = 0f;

    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer >= DestroyAfterSeconds)
        {
            _mParent.GetComponent<SkillSet>().EndAttack();
            Destroy(transform.parent.gameObject);
        }
    }


    public void CollideWith(RaycastHit hit)
    {
        //Debug.Log(hit.collider.gameObject.name);
        _mParent.GetComponent<SkillSet>().EndAttack();
        HealthManager hm = hit.collider.gameObject.GetComponent<HealthManager>();
        if (!hm) return;

        hm.hitBy = _mParent;
        hm.ModifyHealth(-Damage);
        //hm.ReactAt(hit);
    }

    public void Initialize(float damage, GameObject parent)
    {
        Damage = damage;
        _mParent = parent;
    }
}
