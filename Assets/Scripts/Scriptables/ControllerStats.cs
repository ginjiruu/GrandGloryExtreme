using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName ="Single Instances/ControllerStats")]
    public class ControllerStats : ScriptableObject
    {
        public float walkSpeed;
        public float moveSpeed;
        public float sprintSpeed;
        public float rotateSpeed;
        public float rollSpeed;
        public float backstepSpeed;
    }
}