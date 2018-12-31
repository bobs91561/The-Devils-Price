using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{

    public class Inventory : ScriptableObject
    {

        protected List<InventoryItem> items;
        protected List<InventoryItem> equipped;
        protected List<InventoryItem> questItems;


        // Use this for initialization
        void Awake()
        {
            if (items != null) items = new List<InventoryItem>();
            if (questItems != null) questItems = new List<InventoryItem>();
            if (equipped != null) equipped = new List<InventoryItem>();
        }


        public void AddItem(InventoryItem i)
        {
            items.Add(i);
            if (i.QuestItem) questItems.Add(i);
        }

        public void DropItem(InventoryItem i)
        {
            items.Remove(i);
        }
    }
}
