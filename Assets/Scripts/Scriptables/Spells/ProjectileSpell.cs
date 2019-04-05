using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Scriptable
{
    [CreateAssetMenu(menuName = "Actions/Throw Spell")]
    public class ProjectileSpell : SpellAction
    {
        public GameObject projectile;
    }
}