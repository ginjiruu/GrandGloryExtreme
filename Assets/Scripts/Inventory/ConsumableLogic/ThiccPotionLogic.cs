using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class ThiccPotionLogic : MonoBehaviour
    {
        private StatesManager player;

        private void Start()
        {
            Debug.Log("Doing the thing");
            player = GetComponentInParent<StatesManager>();

            player.transform.localScale += Vector3.one;
        }
    }
}