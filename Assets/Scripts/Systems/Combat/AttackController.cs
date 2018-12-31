using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Systems.Combat
{
    /// <summary>
    /// Class for holding basic intializations and updates for generic combat functionalities.
    /// The purpose of these classes is to choose and pass an Attack to the SkillSet.
    /// Children classes will implement choosing and passing. This parent class will update combat resources and provide intilization.
    /// </summary>
    public abstract class AttackController:MonoBehaviour
    {
        protected SkillSet _skillSet;
        protected List<Attack> _attacks;
        protected Dictionary<Attack, float> _cooldowns;

        public float SoulStrength;
        public float DemonicPower;

        public float MaxSoulStrength;
        public float MaxDemonicPower;

        protected void Initialize()
        {
            _skillSet = GetComponent<SkillSet>();
            _cooldowns = new Dictionary<Attack, float>();
            _attacks = _skillSet.attacks;
            SoulStrength = MaxSoulStrength;
            DemonicPower = MaxDemonicPower;
        }

        protected void SendAttack(Attack a)
        {
            _cooldowns[a] = 0f;
            SoulStrength -= a.soulStrengthRequired;
            DemonicPower -= a.demonicPowerRequired;
        }

        void LateUpdate()
        {
            foreach (Attack a in _attacks)
                _cooldowns[a] += Time.deltaTime;
        }

        void OnDisable()
        {
            if (_attacks != null)
                foreach (Attack a in _attacks)
                    _cooldowns[a] = a.coolDown;
        }
    }
}
