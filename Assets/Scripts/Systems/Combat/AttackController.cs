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

        private float m_CoolDownSpeed = 1f;

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
            if (_attacks == null) return;
            foreach (Attack a in _attacks)
                _cooldowns[a] += Time.deltaTime * m_CoolDownSpeed;
        }

        void OnDisable()
        {
            if (_attacks != null)
                foreach (Attack a in _attacks)
                    _cooldowns[a] = a.coolDown;
        }

        protected void SetUpCooldowns()
        {
            foreach(Attack a in _attacks)
            {
                _cooldowns.Add(a, a.coolDown);
            }
        }

        public void RefreshAttacks()
        {
            _cooldowns = new Dictionary<Attack, float>();
            _attacks = _skillSet.attacks;
            SetUpCooldowns();
        }

        public virtual void SetAttacks()
        {
            return;
        }

        #region Levelling Settings
        public void ModifyDemonicPower(float multiplier)
        {
            MaxDemonicPower += MaxDemonicPower * multiplier;
        }

        public void ModifySoulStrength(float multiplier)
        {
            MaxSoulStrength += MaxSoulStrength * multiplier;
        }

        public void ModifyCoolDownSpeed(float multiplier)
        {
            m_CoolDownSpeed += multiplier;
            if (m_CoolDownSpeed <= 0.75f) m_CoolDownSpeed = 0.75f;
        }

        public void ChangeMaxPower(float max)
        {
            MaxDemonicPower = max;
        }

        public void ChangeMaxStrength(float max)
        {
            MaxSoulStrength = max;
        }

        public void AddDemonicPower(float addition)
        {
            ChangeMaxPower(MaxDemonicPower + addition);
        }
        #endregion
    }
}
