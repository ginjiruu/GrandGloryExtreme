using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Scriptable;

namespace SA.Inventory
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    public class Weapon : Item
    {
        public StringVariable oh_idle;
        public StringVariable th_idle;
        public GameObject modelPrefab;
        public ActionHolder[] actions;
        public bool isTwoHanded;

        public OtherPosition lh_positions;

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

    [System.Serializable]
    public class ActionHolder
    {
        public InputType input;
        public Action action;
    }
}

public enum InputType
{
    LightAttack, HeavyAttack, Equiptment, Special, TwoHand
}