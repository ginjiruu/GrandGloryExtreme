using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    [CreateAssetMenu(menuName ="Single Instances/Runtime References")]
    public class RuntimeReferences : ScriptableObject
    {
        public List<Inventory.RuntimeWeapon> runtimeWeapons = new List<Inventory.RuntimeWeapon>();
        public List<Inventory.RuntimeSpecial> runtimeSpecials = new List<Inventory.RuntimeSpecial>();
        public List<Inventory.RuntimeConsumable> runtimeConsumables = new List<Inventory.RuntimeConsumable>();
        public List<Inventory.RuntimeEquipment> runtimeEquipment = new List<Inventory.RuntimeEquipment>();


        public void Init()
        {
            runtimeWeapons.Clear();
        }

        public void RegisterRW(Inventory.RuntimeWeapon rw)
        {
            runtimeWeapons.Add(rw);
        }

        public void UnRegisterRW(Inventory.RuntimeWeapon rw)
        {
            if (runtimeWeapons.Contains(rw))
            {
                if (rw.instance)
                {
                    Destroy(rw.instance);
                }
                runtimeWeapons.Remove(rw);

            }
        }

        public void RegisterRS(Inventory.RuntimeSpecial rs)
        {
            runtimeSpecials.Add(rs);
        }

        public void UnRegisterRS(Inventory.RuntimeSpecial rs)
        {
            if (runtimeSpecials.Contains(rs))
            {
                if (rs.instance)
                {
                    Destroy(rs.instance);
                }
                runtimeSpecials.Remove(rs);

            }
        }

        public void RegisterRC(Inventory.RuntimeConsumable rc)
        {
            runtimeConsumables.Add(rc);
        }

        public void UnRegisterRC(Inventory.RuntimeConsumable rc)
        {
            if (runtimeConsumables.Contains(rc))
            {
                if (rc.instance)
                {
                    Destroy(rc.instance);
                }
                runtimeConsumables.Remove(rc);

            }
        }

        public void RegisterRE(Inventory.RuntimeEquipment re)
        {
            runtimeEquipment.Add(re);
        }

        public void UnRegisterRE(Inventory.RuntimeEquipment re)
        {
            if (runtimeEquipment.Contains(re))
            {
                if (re.instance)
                {
                    Destroy(re.instance);
                }
                runtimeEquipment.Remove(re);

            }
        }
    }

}
