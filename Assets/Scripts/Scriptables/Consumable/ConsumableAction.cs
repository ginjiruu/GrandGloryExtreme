using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Scriptable
{
    [CreateAssetMenu(menuName = "Actions/Consumable Action")]
    public class ConsumableAction : ScriptableObject
    {
        public StringVariable start_anim;
        public bool changeSpeed = false;
        public bool leftHand = false;
        public float animSpeed = 1;
        public GameObject consumable;
    }

}