using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Inventory
{
    [CreateAssetMenu(menuName = "Items/Inventory", order = 0)]
    public class InventoryData : ScriptableObject
    {
        public List<Item> data = new List<Item>();
    }
}
