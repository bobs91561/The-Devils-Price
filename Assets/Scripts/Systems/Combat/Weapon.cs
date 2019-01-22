using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject Hitbox;
        [SerializeField] private float Damage;

        public GameObject MeshEffect;
        public float AdditionalEffectDamage;
        
        private void Start()
        {
            if (MeshEffect) ApplyEffect();
        }

        public GameObject GetHitbox()
        {
            return Hitbox;
        }

        public float GetDamage()
        {
            return Damage;
        }

        /// <summary>
        /// Apply an effect to the weapon
        /// </summary>
        /// <param name="effect"></param>
        public void ApplyEffect(GameObject effect)
        {
            MeshEffect = effect;
            ApplyEffect();
        }

        private void ApplyEffect()
        {
            var go = Instantiate(MeshEffect);
            var me = go.GetComponent<PSMeshRendererUpdater>();
            if (!me)
            {
                Destroy(go);
                return;
            }
            go.transform.parent = transform;
            me.MeshObject = gameObject;
            me.UpdateMeshEffect();
        }
    }
}
