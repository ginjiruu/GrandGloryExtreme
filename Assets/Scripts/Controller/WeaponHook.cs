using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class WeaponHook : MonoBehaviour
    {
        public Collider[] damageColliders;

        public void Init(StatesManager st)
        {
            damageColliders = GetComponentsInChildren<Collider>();
            initColliders(st);
        }

        void initColliders(StatesManager st)
        {
            for (int i = 0; i < damageColliders.Length; i++)
            {
                damageColliders[i].isTrigger = true;
                damageColliders[i].enabled = false;

                DamageCollider d = damageColliders[i].gameObject.AddComponent<DamageCollider>();
                d.onHit = st.HandleDamageCollision;
            }
        }

        public void OpenDamageColliders()
        {
            for (int i = 0; i < damageColliders.Length; i++)
            {
                damageColliders[i].enabled = true;
            }
        }

        public void CloseDamageCOlliders()
        {
            for (int i = 0; i < damageColliders.Length; i++)
            {
                damageColliders[i].enabled = false;
            }
        }
    }
}
