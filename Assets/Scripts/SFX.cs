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
        public List<AudioClip> clips;
        public List<AudioClip> attackSounds;

        private void Start()
        {
            source = GetComponent<AudioSource>();
        }

        public void Step()
        {
            AudioClip c = clips[UnityEngine.Random.Range(0,clips.Count)];
            source.PlayOneShot(c);
        }

        public void Attack()
        {
            AudioClip c = attackSounds[UnityEngine.Random.Range(0, attackSounds.Count)];
            source.PlayOneShot(c);
        }
    }
}
