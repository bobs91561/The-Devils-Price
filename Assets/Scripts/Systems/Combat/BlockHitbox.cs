using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Combat
{
    public class BlockHitbox : MonoBehaviour
    {
        public float BlockRating;
        private float m_BlockIntegrity;

        private GameObject m_Blocker;
        private Weapon m_BlockObject;

        private HealthManager m_HealthManager;
        private SkillSet m_SkillSet;
        private ReactionManager m_ReactionManager;

        private bool m_SuccessfulBlock;
        
        void FixedUpdate()
        {
            m_BlockIntegrity = Mathf.Clamp01(m_BlockIntegrity - Time.fixedDeltaTime);
            //if (m_BlockIntegrity <= 0f) ForceBlockEnd(false);
        }

        public void Initialize(GameObject blocker, float blockRating, Weapon blockingObject = null)
        {
            m_Blocker = blocker;
            m_HealthManager = blocker.GetComponent<HealthManager>();
            m_SkillSet = blocker.GetComponent<SkillSet>();
            m_ReactionManager = blocker.GetComponent<ReactionManager>();
            m_BlockObject = blockingObject;
            BlockRating = blockRating;

            gameObject.layer = blocker.layer;
        }

        private void ManageReactions(Hitbox hb)
        {
            if (m_BlockObject)
            {
                m_BlockObject.UseShield();
            }
            if (m_SuccessfulBlock)
            {
                hb.Blocked();
            }
        }

        /// <summary>
        /// For every hit from a hitbox, decrease the block integrity.
        /// </summary>
        private void DecreaseBlockIntegrity(float damage)
        {
            //Get the ratio of damage to the block rating
            var ratio = damage / BlockRating;

            //If the damage is greater than the blockrating (ratio > 1)
            //  Set block integrity to 0
            if (ratio > 1) m_BlockIntegrity = 0f;
            //Otherwise, decrease the block integrity by the ratio
            else m_BlockIntegrity -= ratio;

            //If the block integrity falls below zero because of a hit, end the block
            if (m_BlockIntegrity <= 0f) ForceBlockEnd(true);
            else m_SuccessfulBlock = true;
            
        }

        /// <summary>
        /// Applies fractional damage to the player based on hit damage and block integrity
        /// </summary>
        private void ApplyDamageToBlocker(float damage)
        {
            //If the damage is less than the block rating and the integrity of the block is above 93%
            //  do no damage to the blocker.
            if (damage < BlockRating && m_BlockIntegrity >= 0.93f) return;

            var appliedDamage = damage - m_BlockIntegrity * BlockRating;
            if (appliedDamage > 0f)
                m_HealthManager.ModifyHealth(-appliedDamage);
        }

        /// <summary>
        /// Force the blocker to end their block.
        /// </summary>
        private void ForceBlockEnd(bool damageDone)
        {
            //Send a "knockback" to the blocker if damage was dealt
            if (damageDone)
            {
                m_ReactionManager.BlockReact();
            }
            //End the block attack
            m_SkillSet.InterruptAttack();
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var hb = collision.gameObject.GetComponent<Hitbox>();
            //Check there is a hitbox or damage-dealing collider involved
            if (!hb) return;

            //Apply any damage
            var damage = Mathf.Abs(hb._mDamageDealt);
            ApplyDamageToBlocker(damage);

            //Decrease the block integrity
            DecreaseBlockIntegrity(damage);

            ManageReactions(hb);
        }

        private void OnTriggerEnter(Collider other)
        {
            var hb = other.gameObject.GetComponent<Hitbox>();
            //Check there is a hitbox or damage-dealing collider involved
            if (!hb) return;

            //Apply any damage
            var damage = Mathf.Abs(hb._mDamageDealt);
            ApplyDamageToBlocker(damage);

            //Decrease the block integrity
            DecreaseBlockIntegrity(damage);

            ManageReactions(hb);
        }

        private void OnParticleCollision(GameObject other)
        {
            var hb = other.GetComponent<Hitbox>();
            //Check there is a hitbox or damage-dealing collider involved
            if (!hb) return;

            //Apply any damage
            var damage = Mathf.Abs(hb._mDamageDealt);
            ApplyDamageToBlocker(damage);

            //Decrease the block integrity
            DecreaseBlockIntegrity(damage);

            ManageReactions(hb);
        }
    }
}
