using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Scriptable;
using SA.Managers;

namespace SA
{
    public class StatesManager : MonoBehaviour
    {
        //Reference to the movement speeds and other variables about this character
        public ControllerStats stats;
        //List of all states that the character can potentially be in
        public States states;
        //List of all the inputs that this states manager can recieve
        public InputVariables inp;
        //The current active model of this states manager
        public GameObject activeModel;

        //contains a reference to all weapons the player has equiped and their runtime equivalents
        public InventoryManager inv_manager;
        //contains the actual actions that the player can perform as dictated by their inventory
        public WeaponManager w_manager;
        //Manager for finding and referencing all currently available weapons and equipment
        [HideInInspector]
        public ResourcesManager r_manager;
        //Reference to all the players gameplat stats such as health, stamina, and type (monster or player
        public PlayerStats pStats;
        //A timer for tracking since the player was hit last
        float timeSinceLastHit;


        // references to the players animator, animator hook, rigidbody, and collider capsule
        #region References
        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public AnimatorHook a_Hook;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public Collider controllerCollider;
        #endregion

        //Layermask for which layers to ignore for raycasting
        [HideInInspector]
        public LayerMask ignoreLayers;
        //Layers to ignore specifically for ground checking
        [HideInInspector]
        public LayerMask ignoreForGroundCheck;

        //Statemanagers reference to the current delta time
        public float delta;
        //This objects specific transform
        public Transform mTransform;
        //Float for tracking time for spellcasting
        float savedTime;
        //for tracking equipment cooldowns
        float equipmentTime;
        EquipmentAction curEquipmentAction;
        //refernce to the current spellcasting action being used
        SpellAction curSpellAction;
        //Reference to the current consumable action being used
        ConsumableAction curConsumableAction;

        //For tracking how much damage the currently inaction attack should do if it hits a target.
        private int curDamage = 25;
        //Reference to the source of an attack for checking if it should hurt a target.
        private PlayerStats.PlayerType curType;
        //Reference to the ammount of uses the held consumable has left
        [SerializeField]
        private int consumableUses;

        //Reference to a characters current state for the purpose of movement control
        public CharState charState;
        public enum CharState
        {
            moving, onAir, armsInteracting, overrideLayerInteracting, roll, dead
        }

        public Image HealthBar;

        #region Init
        //For initializing the statesmanager
        public void Init()
        {
            //loads the resource manager if it isnt already
            if (r_manager == null)
            {
                r_manager = Resources.Load("ResourcesManager") as ResourcesManager;
            }
            mTransform = this.transform;
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);
            ignoreForGroundCheck = ~(1 << 9 | 1 << 10);

            a_Hook = activeModel.AddComponent<AnimatorHook>();
            a_Hook.Init(this);

            //Checks the current inventory and creates a runtime weapon for each
            InitInventory();
            //Checks the weapons it has equiped and changes out the actions on the player for the appropriate actions.
            InitWeaponManager();

            //HealthBar = GameObject.Find("Image");
        }

        public void InitInventory()
        {
            //All Functions check if the player has a wepaoin equiped in that slot and if they do converts it from
            //its inventory version to an individual version with its own stats
            //Init the weapon
            
            if (inv_manager.rh_item)
            {
                Debug.Log("Equiping Right Hand");
                WeaponToRuntime(inv_manager.rh_item, ref inv_manager.rh);
                EquipWeapon(inv_manager.rh, false);
            }

            if (inv_manager.lh_item)
            {
                WeaponToRuntime(inv_manager.lh_item, ref inv_manager.lh);
                EquipWeapon(inv_manager.lh, true);
            }

            //init the special
            if (inv_manager.special_item)
            {
                Debug.Log("Equiping Special");
                SpecialToRuntime(inv_manager.special_item, ref inv_manager.s);
                EquipSpecial(inv_manager.s);
            }

            //init the equipment
            if (inv_manager.equipment_item)
            {
                Debug.Log("Equiping equipment");
                EquipmentToRuntime(inv_manager.equipment_item, ref inv_manager.e);
                EquipEquipment(inv_manager.e);
            }

            //init the consumable
            if (inv_manager.consumable_item)
            {
                Debug.Log("Equiping consumable");
                ConsumableToRuntime(inv_manager.consumable_item, ref inv_manager.c);
                EquipConsumable(inv_manager.c);
            }
        }

        #region xToRuntime
        void WeaponToRuntime(SA.Inventory.Item obj, ref Inventory.RuntimeWeapon slot)
        {
            Inventory.Weapon w = (Inventory.Weapon)obj;
            GameObject go = Instantiate(w.modelPrefab) as GameObject;

            WeaponHook hook = go.AddComponent<WeaponHook>();
            hook.Init(this);

            go.SetActive(false);
            Inventory.RuntimeWeapon rw = new Inventory.RuntimeWeapon();
            rw.instance = go;
            rw.w_actual = w;
            rw.w_hook = hook;

            slot = rw;
            r_manager.runtime.RegisterRW(rw);
        }

        void SpecialToRuntime(SA.Inventory.Item obj, ref Inventory.RuntimeSpecial slot)
        {
            Inventory.Special s = (Inventory.Special)obj;
            GameObject go = Instantiate(s.modelPrefab) as GameObject;

            go.SetActive(false);
            Inventory.RuntimeSpecial rs = new Inventory.RuntimeSpecial();
            rs.instance = go;
            rs.s_actual = s;

            slot = rs;
            r_manager.runtime.RegisterRS(rs);
        }

        void ConsumableToRuntime(SA.Inventory.Item obj, ref Inventory.RuntimeConsumable slot)
        {
            Inventory.Consumables c = (Inventory.Consumables)obj;
            GameObject go = Instantiate(c.modelPrefab) as GameObject;

            go.SetActive(false);
            Inventory.RuntimeConsumable rc = new Inventory.RuntimeConsumable();
            rc.instance = go;
            rc.c_actual = c;
            consumableUses = c.uses;

            slot = rc;
            r_manager.runtime.RegisterRC(rc);

        }

        void EquipmentToRuntime(SA.Inventory.Item obj, ref Inventory.RuntimeEquipment slot)
        {
            Inventory.Equipment e = (Inventory.Equipment)obj;
            GameObject go = Instantiate(e.modelPrefab) as GameObject;
            go.SetActive(false);
            Inventory.RuntimeEquipment re = new Inventory.RuntimeEquipment();
            re.instance = go;
            re.e_actual = e;

            slot = re;
            r_manager.runtime.RegisterRE(re);

        }
        #endregion

        #region EquipX
        void EquipWeapon(Inventory.RuntimeWeapon rw, bool isLeft)
        {
            Vector3 p = Vector3.zero;
            Vector3 e = Vector3.zero;
            Vector3 s = Vector3.one;
            Transform par = null;

            if (isLeft)
            {
                p = rw.w_actual.lh_positions.pos;
                e = rw.w_actual.lh_positions.eulers;
                par = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            }
            else
            {

                par = anim.GetBoneTransform(HumanBodyBones.RightHand);
            }

            rw.instance.transform.parent = par;
            rw.instance.transform.localPosition = p;
            rw.instance.transform.localEulerAngles = e;
            rw.instance.transform.localScale = s;

            rw.instance.SetActive(true);
        }

        void EquipSpecial(Inventory.RuntimeSpecial rs)
        {
            Vector3 p = rs.s_actual.position.pos;
            Vector3 e = rs.s_actual.position.pos;
            Vector3 s = Vector3.one;
            Transform par = null;

            par = anim.GetBoneTransform(HumanBodyBones.Spine);

            rs.instance.transform.parent = par;
            rs.instance.transform.localPosition = p;
            rs.instance.transform.localEulerAngles = e;
            rs.instance.transform.localScale = s;

            rs.instance.SetActive(true);
        }

        void EquipEquipment(Inventory.RuntimeEquipment re)
        {
            Vector3 p = re.e_actual.position.pos;
            Vector3 e = re.e_actual.position.pos;
            Vector3 s = Vector3.one;
            Transform par = null;

            par = anim.GetBoneTransform(HumanBodyBones.LeftHand);

            re.instance.transform.parent = par;
            re.instance.transform.localPosition = p;
            re.instance.transform.localEulerAngles = e;
            re.instance.transform.localScale = s;

            re.instance.SetActive(true);
        }

        void EquipConsumable(Inventory.RuntimeConsumable rc)
        {
            Vector3 p = rc.c_actual.position.pos;
            Vector3 e = rc.c_actual.position.pos;
            Vector3 s = Vector3.one;
            Transform par = null;

            par = anim.GetBoneTransform(HumanBodyBones.Hips);

            rc.instance.transform.parent = par;
            rc.instance.transform.localPosition = p;
            rc.instance.transform.localEulerAngles = e;
            rc.instance.transform.localScale = s;

            rc.instance.SetActive(true);
        }
        #endregion
        public void InitWeaponManager()
        {
            if (inv_manager.lh == null && inv_manager.rh == null)
                return;

            if (inv_manager.rh != null)
            {
                WeaponManager.ActionContainer lightAttack = w_manager.GetAction(InputType.lightAttack);
                lightAttack.action = inv_manager.rh.w_actual.GetAction(InputType.lightAttack);

                WeaponManager.ActionContainer heavyAttack = w_manager.GetAction(InputType.heavyAttack);
                heavyAttack.action = inv_manager.rh.w_actual.GetAction(InputType.heavyAttack);
            }

            if (inv_manager.s != null)
            {
                WeaponManager.ActionContainer specialAttack = w_manager.GetAction(InputType.special);
                specialAttack.action = inv_manager.s.s_actual.GetAction(InputType.special);
            }

            if (inv_manager.c != null)
            {
                WeaponManager.ActionContainer consumable = w_manager.GetAction(InputType.consumable);
                consumable.action = inv_manager.c.c_actual.GetAction(InputType.consumable);
            }

            if (inv_manager.e != null)
            {
                WeaponManager.ActionContainer equipment = w_manager.GetAction(InputType.equipment);
                equipment.action = inv_manager.e.e_actual.GetAction(InputType.equipment);
            }
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                activeModel = anim.gameObject;
            }

            if (anim == null)
                anim = GetComponentInChildren<Animator>();

            anim.applyRootMotion = false;
            anim.GetBoneTransform(HumanBodyBones.LeftHand).localScale = Vector3.one;
            anim.GetBoneTransform(HumanBodyBones.RightHand).localScale = Vector3.one;
        }
        #endregion

        #region Fixed Update
        // Equivalent to fixedupdate
        public void Fixed_Tick(float d)
        {
            delta = d;
            states.onGround = OnGround();

            //pending characterstate do certain things
            switch (charState)
            {
                case CharState.moving:
                    HandleMovement();
                    HandleRotation();
                    break;
                case CharState.onAir:
                    break;
                case CharState.armsInteracting:
                    HandleMovement();
                    HandleRotation();
                    break;
                case CharState.overrideLayerInteracting:
                    rigid.drag = 0;
                    Vector3 v = rigid.velocity;
                    Vector3 tv = inp.animDelta;
                    tv *= 100;
                    tv.y = v.y;
                    rigid.velocity = tv;
                    break;
                case CharState.dead:
                    break;
                default:
                    break;
            }
        }

        //Rotate the character to face the direction of movement
        void HandleRotation()
        {
            Vector3 targetDir = inp.moveDir;

            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = mTransform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(mTransform.rotation, tr, delta * inp.moveAmount * stats.rotateSpeed);
            mTransform.rotation = targetRotation;
        }

        //Move the character proportional to the direction and magnitude of inputa
        void HandleMovement()
        {
            Vector3 v = mTransform.forward;

            if (inp.moveAmount > 0)
                rigid.drag = 0;
            else
                rigid.drag = 4;

            if (states.animIsInteracting)
                v *= inp.moveAmount * stats.walkSpeed;
            else
                v *= inp.moveAmount * stats.moveSpeed;
            //v.y = rigid.velocity.y;
            rigid.velocity = v;
        }
        #endregion

        #region Update
        //Equivalent to Update()
        public void Tick(float d)
        {
            delta = d;
            //Sets the state of the character equal to the result of an onground check
            states.onGround = OnGround();

            //Simple method for determining deadness
            //Needs to be replaced at some point
            if (pStats.health <= 0)
            {
                ChangeState(CharState.dead);
            }
            //If the player has been hit then determine how much longer they have to be invincible to attacks
            if (states.isGettingHit)
            {
                if (Time.realtimeSinceStartup - timeSinceLastHit > .2f)
                {
                    states.isGettingHit = false;
                }
            }

            //Updates player health as decimal for healthbar  
            //float ratio = pStats.health / pStats.maxHealth;
            

            //Based on the character state determine what the player can do
            switch (charState)
            {
                case CharState.moving:
                    bool interact = CheckForInteractionInput();
                    if (!interact)
                        HandleMovementAnim();
                    break;
                case CharState.onAir:
                    break;
                case CharState.armsInteracting:
                    if (states.isSpellcasting)
                    {
                        if (Time.realtimeSinceStartup - savedTime > 1)
                        {
                            states.isSpellcasting = false;
                            PlaySavedSpellAction();
                        }
                    }

                    if(states.isUsingItem)
                    {
                        if (Time.realtimeSinceStartup - savedTime > 1)
                        {
                            states.isUsingItem = false;
                            UseConsumable();
                        }

                    }
                    HandleMovementAnim();

                    break;
                case CharState.overrideLayerInteracting:
                    states.animIsInteracting = anim.GetBool("isInteracting");
                    if (states.animIsInteracting == false)
                    {
                        if(states.hasEquipmentActive)
                        {
                            UseEquipment();
                            states.hasEquipmentActive = false;
                            anim.SetBool("equipment", true);
                        }
                        if (states.isInteracting)
                            states.isInteracting = false;
                        ChangeState(CharState.moving);
                    }
                    break;
                case CharState.roll:
                    states.animIsInteracting = anim.GetBool("isInteracting");
                    if (states.animIsInteracting == false)
                    {
                        if (states.isInteracting)
                            states.isInteracting = false;
                        ChangeState(CharState.moving);
                    }

                    rigid.velocity = inp.moveDir * inp.targetRollSpeed;

                    break;
                default:
                    break;
            }
        }

        //Handeling the interaction input
        bool CheckForInteractionInput()
        {
            //Create an action holder. For each action replace the action holder and play its aciton then move
            // to the next one.
            WeaponManager.ActionContainer a = null;

            if (inp.lightAttack)
            {
                a = GetActionContainer(InputType.lightAttack);
                if (a.action != null)
                    if (a.action.action_obj != null)
                    {
                        //Create an attack action and set it equal to the action holders action object
                        //then set the players current damage with this action equal to that actions damage
                        // and their type equal to that actions damage type
                        AttackAction aa = (AttackAction)a.action.action_obj;
                        curDamage = aa.baseDamage;
                        curType = aa.type;
                        HandleAction(a);
                        return true;
                    }
            }

            if (inp.heavyAttack)
            {
                a = GetActionContainer(InputType.heavyAttack);
                if (a.action != null)
                    if (a.action.action_obj != null)
                    {
                        //Create an attack action and set it equal to the action holders action object
                        //then set the players current damage with this action equal to that actions damage
                        // and their type equal to that actions damage type
                        AttackAction aa = (AttackAction)a.action.action_obj;
                        curDamage = aa.baseDamage;
                        curType = aa.type;
                        HandleAction(a);
                        return true;
                    }
            }

            //TODO Equipment input handling
            if (inp.equipment)
            {
                a = GetActionContainer(InputType.equipment);
                if (a.action != null)
                    if (a.action.action_obj != null)
                    {
                        HandleAction(a);
                        return true;
                    }
            }

            //TODO: Special input handling
            if (inp.special)
            {
                a = GetActionContainer(InputType.special);
                if (a.action != null)
                    if (a.action.action_obj != null)
                    {
                        HandleAction(a);
                        return true;
                    }
            }

            //TODO: Consumable input handling
            if(inp.consumable)
            {
                a = GetActionContainer(InputType.consumable);
                if (a.action != null)
                    if (a.action.action_obj != null)
                    {
                        HandleAction(a);
                        return true;
                    }
            }

            //TODO: Change inputs on twohand
            if (inp.twoHand)
            {
                anim.SetBool("two_handed", !anim.GetBool("two_handed"));
            }


            return false;

        }

        #endregion

        #region Manager Functions


        void HandleAction(WeaponManager.ActionContainer a)
        {
            switch (a.action.actionType)
            {
                case Scriptable.ActionType.attack:
                    AttackAction aa = (AttackAction)a.action.action_obj;
                    PlayAttackAction(a, aa);
                    break;
                case Scriptable.ActionType.consumable:
                    ConsumableAction ca = (ConsumableAction)a.action.action_obj;
                    PlayConsumableAction(a, ca);
                    break;
                case Scriptable.ActionType.equipment:
                    EquipmentAction eq = (EquipmentAction)a.action.action_obj;
                    PlayEquipmentAction(a, eq);
                    break;
                case Scriptable.ActionType.spell:
                    SpellAction sp = (SpellAction)a.action.action_obj;
                    PlaySpellAction(a, sp);
                    break;
                default:
                    break;
            }
        }

        //Helper function for returning the action container ascosiated with the input.
        //
        WeaponManager.ActionContainer GetActionContainer(InputType inp)
        {
            WeaponManager.ActionContainer ac = w_manager.GetAction(inp);
            if (ac == null)
                return null;
            return ac;
        }

        void PlayInteractAnimation(string a)
        {
            anim.CrossFade(a, 0.2f);
            //anim.PlayInFixedTime(a, 5, 0.2f);
        }
        #region Inventory Things
        void PlayAttackAction(WeaponManager.ActionContainer a, AttackAction aa)
        {
            anim.SetBool(StaticStrings.mirror, a.isMirrored);
            PlayInteractAnimation(aa.attack_anim.value);
            if (aa.changeSpeed)
            {
                anim.SetFloat("Speed", aa.animSpeed);
            }
            ChangeState(CharState.overrideLayerInteracting);
        }
        
        void PlayEquipmentAction(WeaponManager.ActionContainer a, EquipmentAction ea)
        {
            /*if (equipmentTime > inv_manager.e.e_actual.coolDown)
            { 
                //Todo create a thing that illustrates you dont have thing yet
                return;
            }*/
            PlayInteractAnimation(ea.start_anim.value);
            if (ea.changeSpeed)
            {
                anim.SetFloat("Speed", ea.animSpeed);
            }
            curEquipmentAction = ea;
            states.hasEquipmentActive = true;
            states.animIsInteracting = true;

            ChangeState(CharState.overrideLayerInteracting);
        }

        void UseEquipment()
        {
            Debug.Log("Using Equipment");
            if(curEquipmentAction is TrapEquipment)
            {
                Debug.Log("Using Trap Equipment");
                TrapEquipment t = (TrapEquipment)curEquipmentAction;
                GameObject go = Instantiate(t.trap);
                go.transform.position = this.transform.position;
                go.transform.rotation = this.transform.rotation;
            }


            states.animIsInteracting = false;
            ChangeState(CharState.moving);
            anim.SetBool("equipment", false);
        }

        void PlayConsumableAction(WeaponManager.ActionContainer a, ConsumableAction ca)
        {
            if (consumableUses == 0)
                return;
            consumableUses--;
            if (ca.changeSpeed)
                anim.SetFloat("Speed", ca.animSpeed);
            PlayInteractAnimation(ca.start_anim.value);
            ChangeState(CharState.armsInteracting);
            curConsumableAction = ca;
            states.animIsInteracting = true;
            states.isUsingItem = true;
        }

        public void UseConsumable()
        {
            Debug.Log("Using Consumable");
            GameObject go = Instantiate(curConsumableAction.consumable);
            go.transform.position = Vector3.zero;

            if (curConsumableAction.leftHand)
                go.transform.parent = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            else
                go.transform.parent = anim.GetBoneTransform(HumanBodyBones.RightHand);
            states.animIsInteracting = false;
            ChangeState(CharState.moving);
        }

        void PlaySpellAction(WeaponManager.ActionContainer a, SpellAction sp)
        {
            string targetAnim = sp.start_anim.value;
            targetAnim += "_l";

            anim.SetBool(StaticStrings.mirror, a.isMirrored);
            anim.CrossFade(targetAnim, 0.2f);
            anim.SetBool(StaticStrings.spellcasting, true);

            if (sp.changeSpeed)
            {
                anim.SetFloat("Speed", sp.animSpeed);
            }

            ChangeState(CharState.armsInteracting);
            states.isSpellcasting = true;
            savedTime = Time.realtimeSinceStartup;
            curSpellAction = sp;
            states.animIsInteracting = true;
        }

        void PlaySavedSpellAction()
        {
            Debug.Log("PlayingSpellActiong");
            anim.SetBool(StaticStrings.spellcasting, false);
            PlayInteractAnimation(curSpellAction.cast_anim.value);
            ChangeState(CharState.overrideLayerInteracting);
            states.animIsInteracting = false;

            CastSpellActual();
        }

        

        //Basic Spell Casting Logic, Projectile trajectory should be completed on the spell projectile object itself
        public void CastSpellActual()
        {
            Debug.Log("Casting Spell");
            if (curSpellAction is ProjectileSpell)
            {
                ProjectileSpell p = (ProjectileSpell)curSpellAction;
                GameObject go = Instantiate(p.projectile);
                Vector3 tp = mTransform.position;
                tp += mTransform.forward;
                tp.y += 1.5f;


                go.transform.position = tp;
                go.transform.rotation = mTransform.rotation;
            }
        }

        #endregion
        void HandleMovementAnim()
        {
            float move = inp.moveAmount;

            if (states.animIsInteracting)
            {
                move = Mathf.Clamp(move, 0, .5f);
            }


            anim.SetFloat(StaticStrings.vertical, move, 0.15f, delta);
        }

        void ChangeState(CharState t)
        {
            charState = t;
            switch (t)
            {
                case CharState.moving:

                    anim.applyRootMotion = false;
                    break;
                case CharState.onAir:

                    anim.applyRootMotion = true;
                    break;
                case CharState.armsInteracting:

                    anim.applyRootMotion = false;
                    break;
                case CharState.overrideLayerInteracting:

                    anim.applyRootMotion = true;
                    anim.SetBool("isInteracting", true);
                    states.isInteracting = true;
                    
                    break;
                case CharState.roll:

                    anim.applyRootMotion = false;
                    anim.SetBool("isInteracting", true);
                    states.isInteracting = true;
                    break;
                default:
                    break;
            }
        }

        public void HandleRoll()
        {
            Vector3 relativeDirection = mTransform.InverseTransformDirection(inp.moveDir);

            float v = relativeDirection.z;
            float x = relativeDirection.x;
            inp.targetRollSpeed = stats.rollSpeed;

            if (relativeDirection == Vector3.zero)
            {
                inp.moveDir = -mTransform.forward;
                inp.targetRollSpeed = stats.backstepSpeed;
            }

            anim.SetFloat(StaticStrings.vertical, v);
            anim.SetFloat(StaticStrings.horizontal, x);

            PlayInteractAnimation("Rolls");
            ChangeState(CharState.roll);
        }

        public void HandleDamageCollision(StatesManager targetStates)
        {
            if (targetStates == this)
                return;

            targetStates.GetHit(curDamage, curType);
        }

        //Used for generic damage like enviromental
        public void GetHit(int Damage)
        {
            Debug.Log("Got Hit");
            if (!states.isGettingHit)
            {
                pStats.health -= (float)Damage;
                PlayInteractAnimation("damage_1");
                ChangeState(CharState.overrideLayerInteracting);
                //SetDamageCollidersStatus(false);
                states.isGettingHit = true;
                timeSinceLastHit = Time.realtimeSinceStartup;
            }
        }

        //used for player specific damage so prevent players from hitting each other
        public void GetHit(int Damage, PlayerStats.PlayerType type)
        {
            Debug.Log("Got Hit");
            if (!states.isGettingHit && type != pStats.playerType)
            {
                pStats.health -= (float)Damage;
                PlayInteractAnimation("damage_1");
                ChangeState(CharState.overrideLayerInteracting);
                //SetDamageCollidersStatus(false);
                states.isGettingHit = true;
                timeSinceLastHit = Time.realtimeSinceStartup;
            }
        }

        public void SetDamageCollidersStatus(bool status)
        {
            if (status)
            {
                inv_manager.rh.w_hook.OpenDamageColliders();
            }
            else
            {
                inv_manager.rh.w_hook.CloseDamageCOlliders();
            }
        }
        #endregion

        bool OnGround()
        {
            bool retVal = false;

            Vector3 origin = mTransform.position;
            origin.y += 0.7f;
            Vector3 dir = -Vector3.up;
            float dis = 1.4f;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreForGroundCheck))
            {
                retVal = true;
                Vector3 targetPosition = hit.point;
                mTransform.position = targetPosition;
            }

            return retVal;
        }
    }
    [System.Serializable]
    public class WeaponManager
    {
        public ActionContainer[] action_containers;

        public ActionContainer GetAction(InputType t)
        {
            for (int i = 0; i < action_containers.Length; i++)
            {
                if (action_containers[i].inp == t)
                    return action_containers[i];
            }

            return null;
        }

        public void Init()
        {
            action_containers = new ActionContainer[4];
            for (int i = 0; i < action_containers.Length; i++)
            {
                ActionContainer a = new ActionContainer();
                a.inp = (InputType)i;
                action_containers[i] = a;
            }
        }

        [System.Serializable]
        public class ActionContainer
        {
            public bool isMirrored;
            public InputType inp;
            public SA.Scriptable.Action action;
        }
    }

    [System.Serializable]
    public class InventoryManager
    {
        public Inventory.RuntimeWeapon rh;
        public Inventory.RuntimeWeapon lh;
        public Inventory.RuntimeEquipment e;
        public Inventory.RuntimeSpecial s;
        public Inventory.RuntimeConsumable c;

        public SA.Inventory.Item rh_item;
        public SA.Inventory.Item lh_item;
        public SA.Inventory.Item equipment_item;
        public SA.Inventory.Item special_item;
        public SA.Inventory.Item consumable_item;

    }

    [System.Serializable]
    public class InputVariables
    {
        public float moveAmount;
        public float horizontal;
        public float vertical;
        public Vector3 moveDir;
        public Vector3 animDelta;

        public bool lightAttack;
        public bool heavyAttack;
        public bool equipment;
        public bool special;
        public bool roll;
        public bool twoHand;
        public bool consumable;

        public float targetRollSpeed;
    }

    [System.Serializable]
    public class States
    {
        public bool onGround;
        public bool isInAcion;
        public bool isAbleToMove;
        public bool isDamageOn;
        public bool isRotateEnabled;
        public bool isAttackEnabled;
        public bool isMoveEnabled;
        public bool isSpellcasting;
        public bool isIKEnabled;
        public bool isUsingItem;
        public bool isLeftHand;
        public bool animIsInteracting;
        public bool isInteracting;
        public bool closeWeapons;
        public bool isInvisible;
        public bool isGettingHit;
        public bool hasEquipmentActive;
    }

    [System.Serializable]
    public class PlayerStats
    {
        public enum PlayerType
        {
            hero, monster
        }

        public PlayerType playerType;
        public float maxHealth = 100;
        public float health = 100;
        public int stamina = 0;
    }
}
