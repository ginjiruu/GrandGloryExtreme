using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Inventory;

namespace SA
{
    public class UI_UpdateSlotFromItem : GameEventListener
    {
        public Image icon;
        public CurItem targetItem;
        public Text text;

        public override void Raise()
        {
            Item item = targetItem.Get();
            if(item == null)
            {
                icon.enabled = false;
                icon.sprite = null;

            }
            else
            {
                icon.sprite = item.ui_info.icon;
                icon.enabled = true;
                text.text = item.ui_info.itemName;
            }

            base.Raise();
        }
    }
}