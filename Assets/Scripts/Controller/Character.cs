using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Inventory;
using System;

namespace SA
{
    public class Character : MonoBehaviour
    {

        public CharacterBody character;
        public InventoryData inventoryData;


        [System.Serializable]
        public class CharacterBody
        {
            //armor Mesh renderers
            public SkinnedMeshRenderer a_chestRenderer;
            public SkinnedMeshRenderer a_headRenderer;
            public SkinnedMeshRenderer a_legsRenderer;
            public SkinnedMeshRenderer a_handsRenderer;

            //character body renderers
            public SkinnedMeshRenderer headRenderer;
            public SkinnedMeshRenderer chestRenderer;
            public SkinnedMeshRenderer legsRenderer;
            public SkinnedMeshRenderer handsRenderer;

        }



        public SkinnedMeshRenderer GetArmorPart(ArmorType type)
        {
            switch (type)
            {
                case ArmorType.Chest:
                    return character.a_chestRenderer;
                case ArmorType.legs:
                    return character.a_legsRenderer;
                case ArmorType.hand:
                    return character.a_handsRenderer;
                case ArmorType.helmet:
                    return character.a_headRenderer;
                default:
                    return null;
            }
        }

        public SkinnedMeshRenderer GetBodyPart(ArmorType type)
        {
            switch (type)
            {
                case ArmorType.Chest:
                    return character.chestRenderer;
                case ArmorType.legs:
                    return character.legsRenderer;
                case ArmorType.hand:
                    return character.handsRenderer;
                case ArmorType.helmet:
                    return character.headRenderer;
                default:
                    return null;
            }
        }

        public void InitArmor()
        {
            for (int i = 0; i < 4; i++)
            {
                SkinnedMeshRenderer r = GetArmorPart((ArmorType)i);
                SkinnedMeshRenderer b = GetBodyPart((ArmorType)i);
                if (b == null || r == null)
                    return;
                b.enabled = true;
                r.enabled = false;
            }
        }

        public void LoadItemsFromData()
        {

            if (inventoryData == null)
                return;

            InitArmor();

            for (int i = 0; i < inventoryData.data.Count; i++)
            {
                WearItem(inventoryData.data[i]);
            }
        }
    


        public void WearItem(Item item)
        {
            if ((item is Armor) == false)
                return;

            Armor a = (Armor)item;
            SkinnedMeshRenderer m = GetBodyPart(a.armorType);
            m.sharedMesh = a.armorMesh;
            m.enabled = true;

            SkinnedMeshRenderer b = GetBodyPart(a.armorType);
            b.enabled = a.baseBodyEnabled;

            /*Material[] newMats = new Material[a.materials.Length];
            for (int i = 0; i < a.materials.Length; i++)
            {
                newMats[i] = a.materials[i];
            }*/

            m.materials = a.materials;
        }

        public void TakeOffItem(ArmorType t)
        {
            SkinnedMeshRenderer m = GetArmorPart(t);
            SkinnedMeshRenderer b = GetBodyPart(t);

            m.enabled = false;
            b.enabled = true;
        }
    }
}
