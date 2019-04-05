using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Scriptable
{
    [System.Serializable]
    public class Action
    {
        public ActionType actionType;
        public Object action_obj;
    }

    public enum ActionType
    {
        attack, consumable, spell, equipment
    }
}