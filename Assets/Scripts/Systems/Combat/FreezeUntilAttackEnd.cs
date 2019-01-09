using Devdog.InventoryPro.UnityStandardAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Systems.Combat
{
    public class FreezeUntilAttackEnd : MonoBehaviour
    {
        private GameObject parent;

        private NavMeshAgent _mAgent;
        private ThirdPersonCharacter _mThirdPerson;
        private AIController _mDecider;

        private DamageForPreSetupObjects damageForPre;
        private DamageForCustomObjects damageForCustom;

        void SetParams()
        {
            damageForCustom = GetComponent<DamageForCustomObjects>();
            parent = damageForCustom.parent;
            _mAgent = parent.GetComponent<NavMeshAgent>();
            _mThirdPerson = parent.GetComponent<ThirdPersonCharacter>();
            _mDecider = parent.GetComponent<AIController>();
            Freeze();
        }
        
        public void Freeze()
        {
            if (_mDecider) _mDecider.Frozen = true;
            if (_mAgent) _mAgent.isStopped = true;
            if (_mThirdPerson) _mThirdPerson.m_IsReacting = true;
        }

        public void Unfreeze()
        {
            if (_mDecider) _mDecider.Frozen = false;
            if (_mAgent) _mAgent.isStopped = false;
            if (_mThirdPerson) _mThirdPerson.m_IsReacting = false;
        }

        public void EndAttack()
        {
            Unfreeze();
        }

        private void OnDestroy()
        {
            Unfreeze();
        }
    }
}
