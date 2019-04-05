using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Inventory;

namespace SA.Managers
{
    [CreateAssetMenu(menuName = "Single Instances/Resource Manager")]
    public class ResourcesManager : ScriptableObject
    {
        public Inventory.Inventory inventory;
        public RuntimeReferences runtime;
        public InventoryData playerInventory;
        public int set;


        public void Init()
        {
            inventory = Resources.Load("inventory") as Inventory.Inventory;
            runtime = Resources.Load("RuntimeReferences") as RuntimeReferences;
            playerInventory = Resources.Load("PlayerInventory") as InventoryData;

            inventory.Init();
            runtime.Init();
        }

        public void CopyInventoryToData(InventoryData inv)
        {
            for (int i = 0; i < inv.data.Count; i++)
            {
                AddItemOnInventory(inv.data[i]);
            }
        }

        public void InitPlayerInventory()
        {
            playerInventory.data.Clear();
        }

        void AddItemOnInventory(string id)
        {
            Item i = GetItem(id);
            AddItemOnInventory(i);
        }

        void AddItemOnInventory(Item i)
        {
            if (i == null)
                return;
            Item newItem = Instantiate(i);
            playerInventory.data.Add(newItem);
        }

        public Inventory.Item GetItem(string id)
        {
            return inventory.GetItem(id);
        }

        public Inventory.Weapon GetWeapon(string id)
        {
            Inventory.Item item = GetItem(id);

            return (Inventory.Weapon)item;
        }

        public Inventory.Armor GetArmor(string id)
        {
            Inventory.Item item = GetItem(id);
            return (Inventory.Armor)item;
        }

        public Inventory.Equipment GetEquipment(string id)
        {
            Inventory.Item item = GetItem(id);
            return (Inventory.Equipment)item;
        }

        public List<Item> GetAllItemsOfType(ItemType type)
        {
            List<Item> retVal = new List<Item>();
            for (int i = 0; i < inventory.all_items.Count; i++)
            {
                Item it = inventory.all_items[i];
                switch (type)
                {
                    case ItemType.weapon:
                        if (it is Weapon)
                            retVal.Add(it);
                        break;
                    case ItemType.armor:
                        if (it is Armor)
                            retVal.Add(it);
                        break;
                    case ItemType.consumable:
                        if (it is Consumables)
                            retVal.Add(it);
                        break;
                    case ItemType.equipment:
                        if (it is Equipment)
                            retVal.Add(it);
                        break;
                    case ItemType.spell:
                        //if (it is )
                           // retVal.Add(it);
                        break;
                    case ItemType.special:
                        if (it is Special)
                            retVal.Add(it);
                        break;
                    default:
                        break;
                }
            }
            return retVal;
        }
    }
    
    
}