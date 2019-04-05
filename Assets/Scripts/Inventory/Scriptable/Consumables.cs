using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Scriptable;

namespace SA.Inventory
{
    [CreateAssetMenu(menuName ="Items/Consumables")]
    public class Consumables : Item
    {
        public GameObject modelPrefab;
        public ActionHolder[] actions;

        public OtherPosition position;

        public int uses;

        public ActionHolder GetActionHolder(InputType inp)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i].input == inp)
                    return actions[i];
            }
            return null;
        }

        public Action GetAction(InputType inp)
        {
            ActionHolder ah = GetActionHolder(inp);
            if (ah == null)
                return null;
            return ah.action;
        }
    }
}

