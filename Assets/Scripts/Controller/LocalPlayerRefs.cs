using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [System.Serializable]
    public class LocalPlayerRefs
    {
        [Header("Items")]
        public CurItem rightHand;
        public CurItem special;
        public CurItem equipment;
        public CurItem consumable;
        public CurItem character;

        [Header("Events")]
        public GameEvent updateGameUI;

    }
}