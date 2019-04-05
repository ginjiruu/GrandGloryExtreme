using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class BeartrapLogic : MonoBehaviour
    {
        private StatesManager player;
        private Rigidbody rb;
        public float time = 4;
        private float elapsed;
        public bool triggered = false;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Maybe Caught Something");
            player = other.GetComponent<StatesManager>();
            if (player == null)
                return;

            if (player.pStats.playerType == PlayerStats.PlayerType.hero)
                return;
            Debug.Log("Caught Something");

            triggered = true;
            rb = player.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
        }

        private void Update()
        {
            if (elapsed < time && triggered == true)
            {
                elapsed += Time.deltaTime;
                rb.velocity = Vector3.zero;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}


