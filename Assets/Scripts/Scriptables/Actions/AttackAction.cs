using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Scriptable
{
    [CreateAssetMenu(menuName = "Actions/Attack Action")]
    public class AttackAction : ScriptableObject
    {
        public PlayerStats.PlayerType type;
        public StringVariable attack_anim;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public int baseDamage = 20;
    }
}
