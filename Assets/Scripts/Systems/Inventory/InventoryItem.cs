using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryItem : ScriptableObject
    {
        public string ItemName;
        public bool QuestItem = false;
        public bool Weapon;
        public bool Consumable;
        public GameObject WorldObject;
        public int Level;
    }
}
