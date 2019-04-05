using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Scriptable
{
    public class EquipmentAction : ScriptableObject
    {
        public StringVariable start_anim;
        public bool changeSpeed = false;
        public float animSpeed = 1;
    }
}