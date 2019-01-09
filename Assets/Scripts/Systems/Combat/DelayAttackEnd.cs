using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Combat
{ public class DelayAttackEnd : MonoBehaviour
    {
        public float DelayForSeconds;
        
        private SkillSet _mSkillSet;
        private DamageForCustomObjects _dm;
        // Start is called before the first frame update
        void Start()
        {
            _dm = GetComponent<DamageForCustomObjects>();
            if (_dm.parent) _mSkillSet = _dm.parent.GetComponent<SkillSet>();
            StartCoroutine("Timer");
        }
       
        IEnumerator Timer()
        {
            yield return new WaitForSeconds(DelayForSeconds);
            if(!_mSkillSet) _mSkillSet = _dm.parent.GetComponent<SkillSet>();

            _mSkillSet.EndAttack();
            SendMessage("EndAttack");
        }

        private void OnDestroy()
        {
            _mSkillSet.EndAttack();
        }
    }
}
