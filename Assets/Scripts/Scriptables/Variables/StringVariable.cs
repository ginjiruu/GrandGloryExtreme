﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Scriptable
{
    [CreateAssetMenu(menuName = "Variables/String")]
    public class StringVariable : ScriptableObject
    {
        public string value;
    }
}