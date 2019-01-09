using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Combat
{
    public class DamageForCustomObjects : MonoBehaviour
    {
        private Hitbox _hb;
        private ExplosiveForce _ef;
        public GameObject parent;

        // Start is called before the first frame update
       
        public void Initialize(float damage, LayerMask mask, GameObject par)
        {
            parent = par;
            _hb = GetComponent<Hitbox>();
            _ef = GetComponent<ExplosiveForce>();
            if (_hb) _hb.Initialize(parent,damage, false, 5f, true);
            if (_ef) _ef.Initialize(parent, damage, mask);
            SendMessage("SetParams");
        }


    }
}
