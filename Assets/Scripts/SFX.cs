using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class SFX:MonoBehaviour
    {
        private AudioSource source;
        public AudioClip WeaponSound;
        public AudioClip DrawWeapon;
        public List<AudioClip> clips;
        public List<AudioClip> attackSounds;
        public List<AudioClip> reactSounds;
        public List<AudioClip> shoutSounds;

        private void Start()
        {
            source = GetComponent<AudioSource>();
            if (!source) source = GetComponentInParent<AudioSource>();
        }

        public void Step()
        {
            if (clips.Count <= 0) return;
            AudioClip c = clips[UnityEngine.Random.Range(0,clips.Count)];
            source.PlayOneShot(c);
        }

        public void Attack()
        {
            if (attackSounds.Count <= 0) return;
            AudioClip c = attackSounds[UnityEngine.Random.Range(0, attackSounds.Count)];
            source.PlayOneShot(c);
        }

        public void DrawWeapons()
        {
            if (!DrawWeapon) return;
            source.PlayOneShot(DrawWeapon);
        }

        public void Shout()
        {
            if (shoutSounds.Count <= 0) return;
            AudioClip c = shoutSounds[UnityEngine.Random.Range(0, shoutSounds.Count)];
            source.PlayOneShot(c);
        }

        public void React()
        {
            if (reactSounds.Count <= 0) return;
            AudioClip c = reactSounds[UnityEngine.Random.Range(0, reactSounds.Count)];
            source.PlayOneShot(c);
        }


        public void PlaySound()
        {
            if (!source.clip && !WeaponSound) return;
            if (!source.clip && WeaponSound) source.clip = WeaponSound;
            source.Play();
        }
    }
}
