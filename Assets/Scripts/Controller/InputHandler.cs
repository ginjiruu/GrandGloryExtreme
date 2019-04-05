using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{

    public class InputHandler : MonoBehaviour
    {

        float horizontal;
        float vertical;
        bool roll;
        bool accept;
        bool interact;
        bool twoHand;

        bool lightAttack;
        bool heavyAttack;
        bool consumable;
        bool special;
        bool equipment;

        public bool changeControllerNumber;

        bool lockon;
        #region controllerManagment
        //private StateManager player;
        private string horizontalAxis;
        private string verticalAxis;
        private string rollInput;
        private string consumableInput;
        private string specialInput;
        private string equipmentInput;
        private string lightInput;
        private string heavyInput;
        private string twoHandInput;
        private string acceptInput;
        [SerializeField]
        private int controllerNumber;
        #endregion
        float delta;

        public GamePhase curPhase;
        public StatesManager states;
        public CameraManager camManager;
        public LocalPlayerRefs localRefs;
        public PlayerPannel playerPannel;
        Transform camTrans;

        #region init
        private void Start()
        {
            InitInGame();
        }

        internal void SetControllerNumber(int number)
        {
            controllerNumber = number;
            horizontalAxis = controllerNumber + "Horizontal";
            verticalAxis = controllerNumber + "Vertical";
            rollInput = controllerNumber + "Roll";
            consumableInput = controllerNumber + "Block";
            specialInput = controllerNumber + "Special";
            equipmentInput = controllerNumber + "Equipment";
            lightInput = controllerNumber + "LightAttack";
            heavyInput = controllerNumber + "HeavyAttack";
            twoHandInput = controllerNumber + "TwoHand";
            acceptInput = controllerNumber + "Accept";
        }

        public void InitInGame()
        {
            states.r_manager = Resources.Load("ResourcesManager") as Managers.ResourcesManager;
            states.r_manager.Init();

            states.Init();
            camManager.Init(states);
            camTrans = camManager.transform;
            UpdateLocalReferences();
        }
        #endregion

        #region fixedupdate
        private void FixedUpdate()
        {
            if (changeControllerNumber)
            {
                Debug.Log("Changing Controller Number");
                SetControllerNumber(controllerNumber);
                changeControllerNumber = false;
            }
            delta = Time.deltaTime;
            GetInput_FixedUpdate();

            switch (curPhase)
            {
                case GamePhase.inGame:
                    InGame_UpdateStates_FixedUpdate();
                    states.Fixed_Tick(delta);
                    break;
                case GamePhase.inMenu:
                    break;
                case GamePhase.inInventory:
                    break;
                default:
                    break;
            }
        }

        

        void GetInput_FixedUpdate()
        {
            vertical = hInput.GetAxis(verticalAxis);
            horizontal = hInput.GetAxis(horizontalAxis);
        }
        void InGame_UpdateStates_FixedUpdate()
        {
            states.inp.vertical = vertical;
            states.inp.horizontal = horizontal;
            states.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(vertical) + Mathf.Abs(horizontal));

            Vector3 moveDir = camTrans.forward * vertical;
            moveDir += camTrans.right * horizontal;
            moveDir.Normalize();
            if (states.charState != StatesManager.CharState.roll)
                states.inp.moveDir = moveDir;
        }
        #endregion

        #region Update

        private void Update()
        {
            delta = Time.deltaTime;
            GetInput_Update();

            switch (curPhase)
            {
                case GamePhase.inGame:
                    InGame_UpdateStates_Update();
                    states.Tick(delta);
                    break;
                case GamePhase.inMenu:
                    InMenue_UpdateStates_Update();
                    break;
                case GamePhase.inInventory:
                    break;
                default:
                    break;
            }
        }

        void GetInput_Update()
        {
            roll = hInput.GetButton(rollInput);
            heavyAttack = hInput.GetButtonDown(heavyInput); 
            lightAttack = hInput.GetButtonDown(lightInput);
            consumable = hInput.GetButtonDown(consumableInput);
            accept = hInput.GetButtonDown(acceptInput);
            interact = hInput.GetButtonDown("Interact");
            twoHand = hInput.GetButtonDown(twoHandInput);
            lockon = hInput.GetButton("LockOn");
            equipment = hInput.GetButtonDown(equipmentInput);
            special = hInput.GetButtonDown(specialInput);
            
        }

        void InGame_UpdateStates_Update()
        {
            states.inp.lightAttack = lightAttack;
            states.inp.heavyAttack = heavyAttack;
            states.inp.equipment = equipment;
            states.inp.special = special;
            states.inp.consumable = consumable;
            states.inp.twoHand = twoHand;
            states.inp.roll = roll;
            states.inp.twoHand = twoHand;

            if (roll)
                states.HandleRoll();
        }

        void InMenue_UpdateStates_Update()
        {
            playerPannel.inp.lightAttack = lightAttack;
            playerPannel.inp.equipment = equipment;
            playerPannel.inp.special = special;
            playerPannel.inp.accept = accept;
            playerPannel.inp.consumable = consumable;
            playerPannel.UpdateMenu();

        }
        #endregion

        #region References
        void UpdateLocalReferences()
        {
            localRefs.rightHand.Clear();
            localRefs.special.Clear();
            localRefs.equipment.Clear();
            localRefs.consumable.Clear();

            localRefs.rightHand.Add(states.inv_manager.rh_item);
            localRefs.special.Add(states.inv_manager.special_item);
            localRefs.equipment.Add(states.inv_manager.equipment_item);
            localRefs.consumable.Add(states.inv_manager.consumable_item);

            localRefs.updateGameUI.Raise();
        }
        #endregion
    }

    public enum InputType
    {
        lightAttack, heavyAttack, equipment, special, roll, twoHand, consumable
    }
    public enum GamePhase
    {
        inGame, inMenu, inInventory
    }

}