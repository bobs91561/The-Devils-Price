using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Systems.Combat
{
    public class Hitbox:MonoBehaviour
    {
        private GameObject _mParent;
        public float _mDamageDealt;
        private Vector3 _mHitboxSize;
        private Collider _collider;

        private bool DestroyAfterHit;
        private float DestroyAfterTime;

        private float timer;

        void Start()
        {
            timer = 0f;
            _collider = GetComponent<Collider>();
            transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= DestroyAfterTime)
                Destroy(gameObject);
        }

        public void Initialize(GameObject obj, float damage,bool afterHit = true, float afterTime = 5f)
        {
            _mParent = obj;
            DestroyAfterHit = afterHit;
            DestroyAfterTime = afterTime;
            gameObject.layer = _mParent.layer;
            _mDamageDealt = damage;
        }

        private void OnDestroy()
        {
            _mParent.GetComponent<SkillSet>().EndAttack();
        }

        private void CheckAggression(GameObject g)
        {
            FriendlyConditional friendly = g.GetComponent<FriendlyConditional>();
            if (!friendly) return;
            friendly.OnAggression();
        }


        void OnCollisionEnter(Collision collision)
        {
            GameObject g = collision.gameObject;
            HealthManager hm = g.GetComponent<HealthManager>();
            if (!hm) return;
            CheckAggression(g);
            hm.hitBy = _mParent;
            hm.ModifyHealth(-_mDamageDealt);
            hm.ReactAt(collision);
            if (DestroyAfterHit) Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject g = other.gameObject;
            HealthManager hm = g.GetComponent<HealthManager>();
            if (!hm) return;
            CheckAggression(g);
            hm.hitBy = _mParent;
            hm.ReactAt(other.transform.position);
            if (DestroyAfterHit) hm.ModifyHealth(-_mDamageDealt);
            else
                hm.ModifyHealth(-_mDamageDealt * Time.deltaTime);

            if (DestroyAfterHit) Destroy(gameObject);
        }

        void OnTriggerStay(Collider other)
        {
            GameObject g = other.gameObject;
            HealthManager hm = g.GetComponent<HealthManager>();
            if (!hm) return;
            CheckAggression(g);

            hm.ModifyHealth(-_mDamageDealt*Time.deltaTime);
            
        }

        void OnParticleCollision(GameObject other)
        {
            GameObject g = other.gameObject;
            HealthManager hm = g.GetComponent<HealthManager>();
            if (!hm) return;
            CheckAggression(g);
            float damagePercent = _mDamageDealt / GetComponent<ParticleSystem>().particleCount;
            hm.ModifyHealth(-damagePercent);
        }

    }
}
