using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class SpeedPotionLogic : MonoBehaviour
    {
        public StatesManager player;
        public float speedBuff;
        public float duration;
        private float elapsed;
        private float oldSpeed;

        private void Start()
        {
            player = GetComponentInParent<StatesManager>();
            oldSpeed = player.stats.moveSpeed;
        }

        private void Update()
        {
            if (elapsed < duration)
            {
                Debug.Log("Doing");
                player.stats.moveSpeed = speedBuff;
                elapsed += Time.deltaTime;
            }
            else
            {
                player.stats.moveSpeed = oldSpeed;
                Destroy(this);
            }
        }
    }
}

