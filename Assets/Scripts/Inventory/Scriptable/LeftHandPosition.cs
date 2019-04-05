using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Inventory
{
    [CreateAssetMenu(menuName ="Items/Other Position")]
    public class OtherPosition : ScriptableObject
    {
        public Vector3 pos;
        public Vector3 eulers;
    }
}

