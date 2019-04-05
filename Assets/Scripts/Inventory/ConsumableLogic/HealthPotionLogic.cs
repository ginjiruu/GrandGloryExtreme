using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class HealthPotionLogic : MonoBehaviour
    {
        public StatesManager player;
        public int healthRestored;
        public float duration;
        private float elapsed;

        private void Start()
        {
            player = GetComponentInParent<StatesManager>();
        }

        private void Update()
        {
            if (elapsed < duration)
            {
                player.pStats.health += healthRestored * Time.deltaTime;
                elapsed += Time.deltaTime;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}

