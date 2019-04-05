using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Inventory;

namespace SA.Managers
{
    public class SessionManager : MonoBehaviour
    {
        public ResourcesManager resourcesManager;

        public GameEvent onPlayerUpdate;

        private void Awake()
        {
            resourcesManager = Resources.Load("ResourcesManager") as ResourcesManager;
            resourcesManager.Init();
        }

        private void Start()
        {
            resourcesManager.InitPlayerInventory();

            onPlayerUpdate.Raise();
        }
    }
}

