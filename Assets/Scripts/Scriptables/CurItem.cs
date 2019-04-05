using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Inventory;

namespace SA
{
    [CreateAssetMenu(menuName = "References/Current Item")]
    public class CurItem : ScriptableObject
    {
        public IntVariable index;
        public List<Item> value = new List<Item>();

        public Item Get()
        {
            if (value.Count == 0)
                return null;

            if (index.value > value.Count - 1)
                index.value = 0;
            return value[index.value];
        }
        public Item GetNext()
        {
            index.value++;
            if (index.value > value.Count - 1)
                index.value = 0;
            return value[index.value];
        }

        public void Add(Item i)
        {
            value.Add(i);
        }

        public void Remove(Item i)
        {
            if (value.Contains(i))
            {
                value.Remove(i);
            }
        }

        public void Clear()
        {
            value.Clear();
            index.value = 0;
        }
    }
}


