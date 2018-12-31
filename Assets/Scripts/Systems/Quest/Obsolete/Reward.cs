using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    public class Reward : ScriptableObject
    {
        public bool XP, Item, Currency;
        public int experience;
        public GameObject item;
        public int currency;

        /// <summary>
        /// Get Rewards for completing a quest
        /// </summary>
        public void GetReward()
        {
            if (XP) GetXP();
            if (Item) GetItem();
            if (Currency) GetCurrency();
        }
        /// <summary>
        /// Add experience pointst to player's experience holder
        /// </summary>
        private void GetXP()
        {

        }
        /// <summary>
        /// Add item to player's inventory
        /// </summary>
        private void GetItem()
        {

        }
        /// <summary>
        /// Add Currency to player's purse
        /// </summary>
        private void GetCurrency()
        {

        }
    }
}
