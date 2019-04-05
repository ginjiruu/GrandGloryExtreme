using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class PlayerPannel : MonoBehaviour
    {
        [SerializeField]
        private InputHandler playerInput;
        public MenuInputVariables inp;
        public StatesManager playerStates;

        private bool hasAccepted = false;
        [Header("Players UI")]
        [SerializeField]
        private Canvas itemCanvas;
        [SerializeField]
        private Image a1;
        [SerializeField]
        private Image a2;
        [SerializeField]
        private Image a3;
        [SerializeField]
        private Image health;

        [Header("All Items of a Type")]
        public CurItem allWeapons;
        public CurItem allSpecials;
        public CurItem allConsumables;
        public CurItem allEquipment;

        [Header("Item Pannels")]
        public UI_UpdateSlotFromItem weaponPannel;
        public UI_UpdateSlotFromItem specialPannel;
        public UI_UpdateSlotFromItem consumablePannel;
        public UI_UpdateSlotFromItem equipmentPannel;

        private bool hasController = false;

        private void Start()
        {
            weaponPannel.targetItem = allWeapons;
            weaponPannel.Raise();
            specialPannel.targetItem = allSpecials;
            specialPannel.Raise();
            consumablePannel.targetItem = allConsumables;
            consumablePannel.Raise();
            equipmentPannel.targetItem = allEquipment;
            consumablePannel.Raise();

            itemCanvas = GetComponent<Canvas>();
            itemCanvas.enabled = false;
        }

        private void Update()
        {
            if(hasAccepted)
            {
                a1.sprite = playerStates.inv_manager.equipment_item.ui_info.icon;
                a2.sprite = playerStates.inv_manager.special_item.ui_info.icon;
                a3.sprite = playerStates.inv_manager.consumable_item.ui_info.icon;
                hasAccepted = false;
            }
            
            float ratio = playerStates.pStats.health / playerStates.pStats.maxHealth;
            health.rectTransform.localScale = new Vector3(ratio, 1, 1);
        }

        public bool HasControllerAssigned()
        {
            return hasController;
        }
        
        public InputHandler AssignedController(int number)
        {
            playerInput.SetControllerNumber(number);
            Debug.Log("Assinging " + playerInput.GetComponentInParent<Transform>().name + " Controller Number " + number); 
            hasController = true;
            playerInput.curPhase = GamePhase.inMenu;
            return playerInput;
        }

        public void UpdateMenu()
        {
            itemCanvas.enabled = true;

            if (inp.lightAttack)
            {
                weaponPannel.targetItem.GetNext();
                weaponPannel.Raise();
            }

            if (inp.consumable)
            {
                consumablePannel.targetItem.GetNext();
                consumablePannel.Raise();
            }

            if (inp.equipment)
            {
                equipmentPannel.targetItem.GetNext();
                equipmentPannel.Raise();
            }

            if (inp.special)
            {
                specialPannel.targetItem.GetNext();
                specialPannel.Raise();
            }

            if(inp.accept)
            {
                //TODO: Change the equipment on the player;
                playerStates.inv_manager.rh_item = weaponPannel.targetItem.Get();
                playerStates.inv_manager.special_item = specialPannel.targetItem.Get();
                playerStates.inv_manager.equipment_item = equipmentPannel.targetItem.Get();
                playerStates.inv_manager.consumable_item = consumablePannel.targetItem.Get();

                playerStates.InitInventory();
                playerStates.InitWeaponManager();
                itemCanvas.enabled = false;
                playerInput.curPhase = GamePhase.inGame;
                if (hasAccepted == false)
                    hasAccepted = true;
            }
        }
    }

    [System.Serializable]
    public class MenuInputVariables
    {
        public bool lightAttack;
        public bool equipment;
        public bool special;
        public bool accept;
        public bool consumable;
    }
}

