using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class WeaponBuffLogic : MonoBehaviour
    {
        private StatesManager player;

        private void Start()
        {
            Debug.Log("Making stick big");
            player = GetComponentInParent<StatesManager>();
            player.inv_manager.rh.instance.transform.localScale += Vector3.one;
        }
    }
}