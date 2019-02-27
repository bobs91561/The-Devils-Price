using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Combat
{
    [RequireComponent(typeof(AudioSource))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject Hitbox;
        [SerializeField] private GameObject BlockHitbox;
        [SerializeField] private float Damage;

        public GameObject MeshEffect;
        public float AdditionalEffectDamage;

        private AudioSource m_source;

        [Tooltip("Sounds made when this weapon hits a character")]public List<AudioClip> hitSounds;
        [Tooltip("Sounds made when this weapon is used as a shield")] public List<AudioClip> shieldSounds;
        private void Start()
        {
            if (MeshEffect) ApplyEffect();
            m_source = GetComponent<AudioSource>();
        }

        public GameObject GetHitbox()
        {
            return Hitbox;
        }

        public GameObject GetBlockHitbox()
        {
            return BlockHitbox;
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

        public void UseWeapon()
        {
            if (!m_source || hitSounds.Count <= 0 || m_source.isPlaying) return;
            AudioClip c = hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)];
            m_source.PlayOneShot(c);
        }

        public void UseShield()
        {

            if (!m_source || shieldSounds.Count <= 0 || m_source.isPlaying) return;
            AudioClip c = shieldSounds[UnityEngine.Random.Range(0, shieldSounds.Count)];
            m_source.PlayOneShot(c);
        }
    }
}
