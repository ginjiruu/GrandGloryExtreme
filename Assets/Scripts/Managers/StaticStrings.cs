using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace SA
{
    public static class StaticStrings
    {
        //Inputs
        public static string Vertical = "Vertical";
        public static string Horizontal = "Horizontal";
        public static string Block = "Block";
        public static string Roll = "Roll";
        public static string Equipment = "Equipment";
        public static string Special = "Special";
        public static string LightAttack = "LightAttack";
        public static string HeavyAttack = "HeavyAttack";
        public static string Pause = "Pause";

        //Animator Parameters
        public static string vertical = "vertical";
        public static string horizontal = "horizontal";
        public static string mirror = "mirror";
        public static string animSpeed = "animSpeed";
        public static string onGround = "onGround";
        public static string two_handed = "two_handed";
        public static string interacting = "interacting";
        public static string canMove = "canMove";
        public static string onEmpty = "OnEmpty";
        public static string spellcasting = "spellcasting";
        public static string enableItem = "enableItem";

        //Animator States
        public static string Rolls = "Rolls";
        public static string attack_interupt = "attack_interupt";
        public static string damage1 = "damage_1";
        public static string damage2 = "damage_2";
        public static string damage3 = "damage_3";
        public static string changeWeapon = "changeWeapon";
        public static string emptyBoth = "Empty Both";
        public static string emptyLeft = "Empty Left";
        public static string emptyRight = "Empty Right";
        public static string equipWeapon_oh = "equipWeapon_oh";
        public static string pick_up = "pick_up";

        //UIU INteractions
        public static string ui_ac_pick
        {
            get { return "Pick up item: " + Roll; }
        }

        //Other
        public static string _1 = "_1";
        public static string _r = "_r";
    }
}