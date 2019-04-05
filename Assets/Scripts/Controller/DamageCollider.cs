using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class DamageCollider : MonoBehaviour
    {
        public delegate void OnHit(StatesManager statees);
        public OnHit onHit;

        private void OnTriggerEnter(Collider other)
        {
            StatesManager states = other.transform.GetComponentInParent<StatesManager>();
            if (states != null)
            {
                Debug.Log("states not null");
                if (onHit != null)
                {
                    onHit(states);
                }
            }
        }
    }
}