using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //initializing variables and objects


    //PHYSICS
    Controller2D controller;    //for collision detection and movement control

    public float jumpHeight = 4;        //jump height (set through Inspector)
    public float timeToJumpApex = 0.4f; //time to reach jump height (set through Inspector)
    float gravity;          //gravity (calculated through kinematic eqtns)
    float jumpVelocity;     //initial velocity when jumping (calculated through kinematic eqtns)


    //LOCOMOTION
    Vector3 velocity;       //velocity x and y

    float moveSpeed = 6;   //horizontal move speed

    bool isMoveLeft;        //is moving left or not
    bool isMoveRight;       //is moving right or not

    bool isJump = false;            //is jumping or not
    bool doubleJump = false;        //can double jump or not

    float accelTimeInAir = 0.4f;        //acceleration damper time in air (harder to manuveur horizontally in air)
    float accelTimeOnGround = 0.1f;     //acceleration damper time on ground
    float targetVelocityX;              //target velocity (set by input.x)
    float velocityXDamper;              //used for velocity x damping (smooth start end)

    bool isSlide = false;

    bool isHit = false;             //is collided with enemy or not


    //PLAYER STATS
    public PlayerStats stats;

    public float size;      //size of player (used for Flip())
    public float health;     //hit points (calculated from Vitality Stat)


    //ENEMY STATS
    GameObject Enemy;
    EnemyStats enemyStats;


    //ANIMATION
    Animator anim;          //Animator Controller


    //WEAPONS
    public ReserveEquipment reserveEquipment;
    public GameObject equipment;

    public int currentWeapon = 100;      //current weapon (retrieved from equipments)
    public List<GameObject> weaponsQuickSlotted;     //list of weapons in hotbar (assigned through Inspector)

    WeaponAttack weaponAttack;
    EquipmentStats weaponStats;
    //public String weaponType;

    public bool isEquip = false;
    public GameObject shealthWeapon;
    public Animator animShealth;


    //HUD
    PlayerHUD HUD;
    public Hotbar hotbar;

    public StatsWindowAssignment statWindowAssignment;
    public StatsWindowAssignmentWeapon statWindowAssignmentWeapon;
    

    //DIALOGUE
    public TextController txtController;


    //INVENTORY and EQUIPMENT
    public PlayerInventoryController playerInventoryController;
    public PlayerEquipmentController playerEquipmentController;
    public List<List<GameObject>> listEquipped;
    public List<GameObject> listEquippedWeapons;

    bool isInventory = false;
    bool isEquipment = false;

    bool isSlowMo = false;
    float timeDamper;

    


    /// <summary>
    /// initialization
    /// </summary>
    void Start()
    {
        //get components
        controller = GetComponent<Controller2D>();
        weaponAttack = GetComponentInChildren<WeaponAttack>();
        anim = GetComponent<Animator>();
        HUD = GetComponent<PlayerHUD>();

        //kinematic calculations
        gravity = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        //calculate health from Vitality Stat
        health = stats.playerStats.Vitality.BaseValue * 10;
        
        transform.localScale = new Vector3(size, size, size);
        
        listEquippedWeapons = playerEquipmentController.equipmentList[0];

        weaponAttack = weaponsQuickSlotted[currentWeapon].gameObject.GetComponent<WeaponAttack>();

        animShealth.SetBool("isShealth", true);

        currentWeapon = 100;

    }

    

    /// <summary>
    /// controls
    /// </summary>
    void Update()   //runs with every frame
    {

        //CONTROL: [LOCO] MOVE
        Vector2 input = (!isInventory) ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) : new Vector2(0,0);  //gets player movement controls
        //Vector2 input = (controller.collisions.isSlidingDownSlope) ? new Vector2(1 * controller.collisions.slopeNormalDirection, 0) : (!isInventory) ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) : new Vector2(0, 0); //get player movement

        //flip the player horizontally
        if (input.x != 0 && !isHit)
        {
            Flip();
        }

        //CONTROL: [LOCO] JUMP
        if (Input.GetKeyDown(KeyCode.Space))            //jump when Spacebar is pressed (*see FixedUpdate())
        {
            if (controller.collisions.below == true)    //if on ground/platform
            {
                isJump = true;                          //is jumping
                doubleJump = true;                      //able to double jump
            }
            else if (doubleJump == true)                //if spacebar is pressed when not on ground but double jump is true
            {
                doubleJump = false;                     //reset double jump
                isJump = true;                          //is jumping
            }
        }

        //CONTROL: [LOCO] SPRINT
        moveSpeed = (Input.GetKey(KeyCode.E)) ? 24 : 12;    //increase move speed when E is pressed

        //CONTROL: [ATTACK] BASIC
        if (Input.GetKeyDown(KeyCode.F) && !weaponAttack.isAttacking && isEquip && controller.collisions.below && !isHit)        //attack when F is pressed 
        {
            anim.SetTrigger("atk_basic");       //trigger attacking with melee animation
            listEquippedWeapons[currentWeapon].GetComponentInChildren<Animator>().SetTrigger("basic");
            if (controller.collisions.below)
            {
                velocity.x -= (Mathf.Abs(velocity.x) > 0.8) ? 0.6f * Mathf.Sign(velocity.x) : 0;    //slow down velocity x temporarily when moving and attacking
                velocity.y += (Mathf.Abs(velocity.x) > 0.8) ? 10 : 0;                               //jump slightly when moving and attacking
            }
        }

        //CONTROL: [LOCO] SLIDE
        if (Input.GetKeyDown(KeyCode.S) && (Mathf.Abs(velocity.x) > 0.1) && !isSlide && controller.collisions.below)
        {
            anim.SetBool("isSlide", true);
            isSlide = true;
            StartCoroutine(SlideDuration(1f));
        }

        if (isSlide) {
            velocity.x = Mathf.SmoothDamp(Mathf.Sign(velocity.x) * 20, 0, ref velocityXDamper, 0.2f);
            controller.CalculateRaySpacing();
        }

        //CONTROL: TOGGLE EQUIP
        if (Input.GetKeyDown(KeyCode.R) && currentWeapon < listEquippedWeapons.Count)
        {
            toggleEquip();
        }

        //CONTROL: EQUIP HOTBAR ITEMS
        for (int i = 0; i < listEquippedWeapons.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && !isEquip && listEquippedWeapons[i] != playerInventoryController.pu_blank)
            {
                changeWeapon(i);
            }
        }

        //CONTROL: [CHEAT] LIFE UP
        if (Input.GetKeyDown(KeyCode.L))
        {
            health = stats.playerStats.Vitality.BaseValue * 10;
            HUD.showHealth(health); //refresh health bar
        }

        //CONTROL: GUI: Stats Window
        if (Input.GetKeyDown(KeyCode.C)) {
            HUD.toggleStatsWindow(isEquip, true);
        }

        //CONTROL: GUI: Inventory Window
        if (Input.GetKeyDown(KeyCode.I)) {
            isInventory = !isInventory;
            HUD.toggleInventoryWindow();
            if (!isInventory && isEquipment) { HUD.toggleEquipmentWindow(); isEquipment = false; }
            playerInventoryController.equipMode = isEquipment;
            isSlowMo = !isSlowMo;
        }

        //CONTROL: GUI: Equipment Window
        if (Input.GetKeyDown(KeyCode.O))
        {
            isEquipment = !isEquipment;
            if (isInventory != isEquipment && isEquipment == true) { HUD.toggleInventoryWindow(); }
            HUD.toggleEquipmentWindow();
            isInventory = (isEquipment) ? true : isInventory;
            playerInventoryController.equipMode = isEquipment;
            isSlowMo = isInventory;
        }

        //time control (slow motion/pause)
        if (isSlowMo)
        {
            Time.timeScale = (Time.timeScale > 0.015f) ? Mathf.SmoothDamp(Time.timeScale, 0, ref timeDamper, 0.1f) : 0.01f;
        }
        else if (Time.timeScale != 1) {
            Time.timeScale = 1;
        }

        

#if UNITY_ANDROID
        /*if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            touchDeltaPosition.x = Mathf.Clamp(touchDeltaPosition.x, -1, 1);

            targetVelocityX = touchDeltaPosition.x * moveSpeed;

            Flip(velocity);
        

        if (isMoveLeft) {
            targetVelocityX = -1 * moveSpeed;
            Flip(velocity);
        } else if (isMoveRight){
            targetVelocityX = 1 * moveSpeed;
            Flip(velocity);
        } else {
            targetVelocityX = 0;
        }}
        public void Left()
        {
            isMoveLeft = true;
        }

        public void Right()
        {
            isMoveRight = true;
        }

        public void Jump()
        {
            isJump = true;
        }*/
#endif

        //get input for velocity
        if (input.x != 0) { targetVelocityX = input.x * moveSpeed; } else { targetVelocityX = 0; }  //if there is horizontal input, set target velocity x to input multiplied by move speed

        //setting velocity
        if (!isHit && !isSlide)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXDamper, (controller.collisions.below) ? accelTimeOnGround : accelTimeInAir);    //damp velocity x (smooth)
        }

        if (controller.collisions.isSlidingDownSlope) {  //accouting for slope friction
            velocity.y += (1/controller.collisions.slopeNormal.y) * gravity * Time.deltaTime;
        } else {
            velocity.y += gravity * Time.deltaTime;     //what goes up must come down
        }

        //checking for collisions
        controller.Move(velocity * Time.deltaTime); //see Controller2D.cs
        
        anim.SetFloat("runSpeed", Mathf.Abs(input.x) * moveSpeed);  //set animation speed of running according to velocity x
        animShealth.SetFloat("runSpeed", Mathf.Abs(input.x) * moveSpeed);
    }



    /// <summary>
    /// jumping physics, animation controls
    /// </summary>
    void FixedUpdate()  //runs at a determined interval, more reliable and used for physics
    {
        //actual jumping
        if (isJump)
            if (controller.collisions.isSlidingDownSlope) { //sliding down slope jumping
                
                velocity.y = jumpVelocity / 2;
                velocity.x = moveSpeed * 2 * Mathf.Sign(controller.collisions.slopeNormal.x);
                doubleJump = false; //restrict double jumping after jumping from sliding down slope

            } else {

                anim.SetTrigger("Jump");    //trigger jump animation
                isJump = false;             //reset is jumping
                velocity.y = jumpVelocity;  //give initial velocity boost to y

        }
        else if (controller.collisions.below && !controller.collisions.isSlidingDownSlope)   //if not is jumping and is on ground, set velocity y to 0 (so gravity doesn't acculmulate)
        {
            velocity.y = 0;
        }

        velocity.y -= (controller.collisions.above) ? jumpVelocity / 2 : 0; //if collide above, give downward velocity y boost (bounce off ceilings faster)

        //animation transitions
        anim.SetBool("collisions.below", (controller.collisions.below) ? true : false);
        anim.SetFloat("directionY", Mathf.Sign(velocity.y));
        
        anim.SetFloat("velocityY", velocity.y);

        anim.SetBool("isClimbingSlope", (controller.collisions.isClimbingSlope) ? true : false);
        anim.SetBool("isDescendingSlope", (controller.collisions.isDescendingSlope) ? true : false);

        animShealth.SetBool("collisions.below", (controller.collisions.below) ? true : false);
        animShealth.SetFloat("directionY", Mathf.Sign(velocity.y));
    }



    /// <summary>
    /// flip player horizontally
    /// </summary>
    void Flip()
    {
        if (Mathf.Sign(velocity.x) == 1)
        {
            transform.localScale = new Vector3(-size, size, size);
            HUD.flipHUD(Mathf.Sign(velocity.x));
        }
        else if (Mathf.Sign(velocity.x) == -1)
        {
            transform.localScale = new Vector3(size, size, size);
            HUD.flipHUD(Mathf.Sign(velocity.x));
        }
    }



    /// <summary>
    /// switch shealthed weapon by:
    ///     1. enable weapon
    ///     2. get weaponAttack script
    ///     3. get sprite
    ///     4. get weapon type (tag)
    ///     5. disable weapon
    /// </summary>
    /// <param name="weaponSlot"></param>
    void changeWeapon(int weaponSlot)   //change equipped weapon by setting them active and inactive
    {
        //get reference to current weapon
        currentWeapon = weaponSlot;     //get index of current weapon

        //set current weapon to active to get sprite and weapon type
        listEquippedWeapons[currentWeapon].gameObject.SetActive(true);
        weaponAttack = listEquippedWeapons[currentWeapon].gameObject.GetComponent<WeaponAttack>();  //get weapon object's script

        //activate shealth and put weapon sprite into shealth
        animShealth.SetBool("isShealth", true);
        shealthWeapon.GetComponent<SpriteRenderer>().sprite = listEquippedWeapons[currentWeapon].gameObject.GetComponent<SpriteRenderer>().sprite;

        //get weapon type
        //weaponType = weapons[currentWeapon].gameObject.tag;

        //finished with current weapon and therefore disable it
        listEquippedWeapons[currentWeapon].gameObject.SetActive(false);
    }



    /// <summary>
    /// collision with enemies/objects
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    IEnumerator OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && isHit == false)     //if collided with enemies
        {
            //get reference to enemy and its stats
            Enemy = other.gameObject;
            enemyStats = Enemy.GetComponent<EnemyStats>();

            //set hit marker to true (to prevent reading the same hit multiple times)
            isHit = true;

            //execute hit action
            Hit(enemyStats.enemyStats.Power.BaseValue);

            //hit protection/reaction time
            yield return new WaitForSeconds(0.2f);
            velocity.x = 0;
            yield return new WaitForSeconds(0.2f);

            //reset hit marker
            isHit = false;
        }
        else if (other.gameObject.CompareTag("Pickups"))     //if collided with enemies
        {
            //add pickup to weapon list by finding reference by name
            //weapons.Add(equipments.transform.Find(other.gameObject.name.Replace("pu_", "")).gameObject);

            //add pickup to inventory
            for (int i = 0; i < reserveEquipment.listReserveEquipment.Count; i++)
            {
                for (int j = 0; j < reserveEquipment.listReserveEquipment[i].Count; j++)
                {
                    if (reserveEquipment.listReserveEquipment[i][j].gameObject.name == other.gameObject.name.Replace("pu_", ""))
                    {
                        GameObject obj = Instantiate(reserveEquipment.listReserveEquipment[i][j], Vector3.zero, Quaternion.identity);
                        playerInventoryController.addToInventory(obj);

                        //check weapon or armor
                        if (obj.tag == "Armor")
                        {
                            //changing parent
                            obj.transform.SetParent(equipment.transform);
                            obj.SetActive(false);
                            break;
                        }
                        else if (obj.tag == "Weapon")
                        {

                            //changing parent
                            Vector3 localPosition = obj.transform.localPosition;
                            Quaternion localRotation = obj.transform.localRotation;
                            Vector3 localScale = obj.transform.localScale;
                            obj.transform.SetParent(equipment.transform.Find(obj.GetComponent<EquipmentStats>().equipmentStats.Type));
                            obj.transform.localPosition = localPosition;
                            obj.transform.localRotation = localRotation;
                            obj.transform.localScale = localScale;
                            obj.active = false;
                            break;
                        }
                    }
                }
            }

            //add pickup to hotbar
            //hotbar.updateHotbar(weapons.Count, other.gameObject.GetComponent<SpriteRenderer>().sprite);

            //destroy pickup
            Destroy(other.gameObject);
        }
    }



    /// <summary>
    /// when in npc dialogue range
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("NPC") && !txtController.isInDialogueProximity)
        {
            txtController.isInDialogueProximity = true;
            txtController.startDialogue(other.gameObject.GetComponent<TextHolder>().giveMeTextList(UnityEngine.Random.Range(0, other.gameObject.GetComponent<TextHolder>().textAssetList.Count)));
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("NPC")) {
            txtController.isInDialogueProximity = false;
        }
    }



    /// <summary>
    /// apply damage from enemy
    /// </summary>
    /// <param name="damage"></param>
    void Hit(float damage)  //when hit by enemies
    {
        if (isHit)
        {
            //apply damage to player
            health -= damage;

            //update health bar
            HUD.showHealth(health);

            //run animation
            anim.SetTrigger("isHurt");

            //move player to flinch backwards
            velocity.x -= Mathf.Sign(Enemy.transform.position.x - gameObject.transform.position.x) * 15;

            //kill player when health is below threshold
            if (health <= 0)
            {
                anim.SetBool("isDead", true);
                //gameObject.SetActive(false);    //set player to inactive when health reaches zero
            }
        }
    }



    /// <summary>
    /// time allocated for sliding animation
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    IEnumerator SlideDuration(float sec) {
        yield return new WaitForSeconds(sec);
        anim.SetBool("isSlide", false);
        isSlide = false;
        controller.CalculateRaySpacing();
    }



    /// <summary>
    /// equip/dequip weapons (get from and put back into shealth)
    /// </summary>
    void toggleEquip() {

        //toggle equip state
        isEquip = !isEquip;

        //toggle shealth active state
        shealthWeapon.active = !shealthWeapon.active;

        //toggle in hand weapon active state
        Debug.Log(listEquippedWeapons[currentWeapon].transform.parent);
        listEquippedWeapons[currentWeapon].gameObject.active = !listEquippedWeapons[currentWeapon].gameObject.active;

        //toggle shealth animation
        animShealth.SetBool("isShealth", !isEquip);

        //get weapon stats
        weaponStats = listEquippedWeapons[currentWeapon].GetComponent<EquipmentStats>();
        stats.playerStats.Power.BaseValue += weaponStats.equipmentStats.Power.BaseValue * ((isEquip) ? 1 : -1);
        stats.playerStats.Knockback.BaseValue += weaponStats.equipmentStats.Knockback.BaseValue * ((isEquip) ? 1 : -1);

        //update player stat window
        statWindowAssignment.updateStatWindow(true, weaponStats.equipmentStats.Power.StatName);

        //update weapon stat window by activating it, update, and then deactivating it if 
        HUD.toggleStatsWindow(true, false);
        statWindowAssignmentWeapon.updateStatWindow(weaponStats.equipmentStats.Power.StatName, currentWeapon);
        statWindowAssignmentWeapon.updateStatWindow(weaponStats.equipmentStats.Knockback.StatName, currentWeapon);
        HUD.toggleStatsWindow(isEquip, false);
    }



    public void dequipArmor(GameObject armor) {
        IDictionary<string, bool> equippedSlots = playerInventoryController.equipmentController.equippedSlots;
        switch (armor.gameObject.GetComponent<EquipmentStats>().equipmentStats.Slot)
        {
            case 0:
                armor.transform.SetParent(equipment.transform.Find("Armor(inventory)"));
                armor.SetActive(false);
                equippedSlots["torso"] = false;
                break;
            case 1:
                armor.transform.SetParent(equipment.transform.Find("Armor(inventory)"));
                GameObject rightArm = this.transform.Find("right").Find("right arm").GetChild(0).gameObject;
                rightArm.transform.SetParent(armor.transform);
                GameObject leftArm = this.transform.Find("left").Find("left arm").GetChild(0).gameObject;
                leftArm.transform.SetParent(armor.transform);
                armor.SetActive(false);
                rightArm.SetActive(false);
                leftArm.SetActive(false);
                equippedSlots["torso"] = false;
                equippedSlots["rightArm"] = false;
                equippedSlots["leftArm"] = false;
                break;
            case 2:
                armor.transform.SetParent(equipment.transform.Find("Armor(inventory)"));
                GameObject rightHand = this.transform.Find("right").Find("right bottom arm").GetChild(0).gameObject;
                rightHand.transform.SetParent(armor.transform);
                GameObject leftHand = this.transform.Find("left").Find("left bottom arm").GetChild(0).gameObject;
                leftHand.transform.SetParent(armor.transform);
                rightHand.SetActive(false);
                leftHand.SetActive(false);
                equippedSlots["rightHand"] = false;
                equippedSlots["leftHand"] = false;
                break;
            case 3:
                armor.transform.SetParent(equipment.transform.Find("Armor(inventory)"));
                GameObject rightThigh = this.transform.Find("right").Find("right thigh").GetChild(0).gameObject;
                rightThigh.transform.SetParent(armor.transform);
                GameObject leftThigh = this.transform.Find("left").Find("left thigh").GetChild(0).gameObject;
                leftThigh.transform.SetParent(armor.transform);
                rightThigh.SetActive(false);
                leftThigh.SetActive(false);
                equippedSlots["rightThigh"] = false;
                equippedSlots["leftThigh"] = false;
                break;
            case 4:
                armor.transform.SetParent(equipment.transform.Find("Armor(inventory)"));
                rightThigh = this.transform.Find("right").Find("right thigh").GetChild(0).gameObject;
                rightThigh.transform.SetParent(armor.transform);
                leftThigh = this.transform.Find("left").Find("left thigh").GetChild(0).gameObject;
                leftThigh.transform.SetParent(armor.transform);
                GameObject rightLeg = this.transform.Find("right").Find("right leg").GetChild(0).gameObject;
                rightLeg.transform.SetParent(armor.transform);
                GameObject leftLeg = this.transform.Find("left").Find("left leg").GetChild(0).gameObject;
                leftLeg.transform.SetParent(armor.transform);
                rightThigh.SetActive(false);
                leftThigh.SetActive(false);
                rightLeg.SetActive(false);
                leftLeg.SetActive(false);
                equippedSlots["rightThigh"] = false;
                equippedSlots["leftThigh"] = false;
                equippedSlots["rightLeg"] = false;
                equippedSlots["leftLeg"] = false;
                break;
            case 5:
                armor.transform.SetParent(equipment.transform.Find("Armor(inventory)"));
                rightLeg = this.transform.Find("right").Find("right thigh").GetChild(0).gameObject;
                rightLeg.transform.SetParent(armor.transform);
                leftLeg = this.transform.Find("left").Find("left leg").GetChild(0).gameObject;
                leftLeg.transform.SetParent(armor.transform);
                rightLeg.SetActive(false);
                leftLeg.SetActive(false);
                equippedSlots["rightLeg"] = false;
                equippedSlots["leftLeg"] = false;
                break;
            default:
                break;
        }
    }

    public void equipArmor(GameObject armor) {
        armor.SetActive(true);
        IDictionary<string, bool> equippedSlots = playerInventoryController.equipmentController.equippedSlots;
        //transform equip
        switch (armor.gameObject.GetComponent<EquipmentStats>().equipmentStats.Slot)
        {
            case 0:
                if (!equippedSlots["torso"])
                {
                    //torso
                    armor.transform.SetParent(this.transform.Find("torso"));
                    armor.transform.localPosition = Vector3.zero;
                    armor.transform.localScale = new Vector3(1, 1, 1);

                    armor.SetActive(true);
                    equippedSlots["torso"] = true;
                }
                break;
            case 1:
                if (!equippedSlots["torso"] && !equippedSlots["rightArm"] && !equippedSlots["leftArm"])
                {
                    //torso
                    armor.transform.SetParent(this.transform.Find("torso"));
                    armor.transform.localPosition = Vector3.zero;
                    armor.transform.localScale = new Vector3(1, 1, 1);
                    //right arm
                    GameObject rightArm = armor.transform.Find("rightArm").gameObject;
                    rightArm.transform.SetParent(this.transform.Find("right").Find("right arm"));
                    rightArm.transform.localPosition = Vector3.zero;
                    rightArm.transform.localScale = new Vector3(1, 1, 1);
                    //leftarm
                    GameObject leftArm = armor.transform.Find("leftArm").gameObject;
                    leftArm.transform.SetParent(this.transform.Find("left").Find("left arm"));
                    leftArm.transform.localPosition = Vector3.zero;
                    leftArm.transform.localScale = new Vector3(1, 1, 1);

                    armor.SetActive(true);
                    rightArm.SetActive(true);
                    leftArm.SetActive(true);
                    equippedSlots["torso"] = true;
                    equippedSlots["rightArm"] = true;
                    equippedSlots["leftArm"] = true;
                }
                break;
            case 2:
                if (!equippedSlots["rightHand"] && !equippedSlots["leftHand"])
                {
                    //right hand
                    GameObject rightHand = armor.transform.Find("rightHand").gameObject;
                    rightHand.transform.SetParent(this.transform.Find("right").Find("right bottom arm"));
                    rightHand.transform.localPosition = Vector3.zero;
                    rightHand.transform.localScale = new Vector3(1, 1, 1);
                    //left hand
                    GameObject leftHand = armor.transform.Find("leftHand").gameObject;
                    leftHand.transform.SetParent(this.transform.Find("left").Find("left bottom arm"));
                    leftHand.transform.localPosition = Vector3.zero;
                    leftHand.transform.localScale = new Vector3(1, 1, 1);

                    rightHand.SetActive(true);
                    leftHand.SetActive(true);
                    equippedSlots["rightHand"] = true;
                    equippedSlots["leftHand"] = true;
                }
                break;
            case 3:
                if (!equippedSlots["rightThigh"] && !equippedSlots["leftThigh"])
                {
                    //right thigh
                    GameObject rightThigh = armor.transform.Find("rightThigh").gameObject;
                    rightThigh.transform.SetParent(this.transform.Find("right").Find("right thigh"));
                    rightThigh.transform.localPosition = Vector3.zero;
                    rightThigh.transform.localScale = new Vector3(1, 1, 1);
                    //left thigh
                    GameObject leftThigh = armor.transform.Find("leftThigh").gameObject;
                    leftThigh.transform.SetParent(this.transform.Find("left").Find("left thigh"));
                    leftThigh.transform.localPosition = Vector3.zero;
                    leftThigh.transform.localScale = new Vector3(1, 1, 1);

                    rightThigh.SetActive(true);
                    leftThigh.SetActive(true);
                    equippedSlots["rightThigh"] = true;
                    equippedSlots["leftThigh"] = true;
                }
                break;
            case 4:
                if (!equippedSlots["rightThigh"] && !equippedSlots["leftThigh"] && !equippedSlots["rightLeg"] && !equippedSlots["leftLeg"])
                {
                    //right thigh
                    GameObject rightThigh = armor.transform.Find("rightThigh").gameObject;
                    rightThigh.transform.SetParent(this.transform.Find("right").Find("right thigh"));
                    rightThigh.transform.localPosition = Vector3.zero;
                    rightThigh.transform.localScale = new Vector3(1, 1, 1);
                    //left thigh
                    GameObject leftThigh = armor.transform.Find("leftThigh").gameObject;
                    leftThigh.transform.SetParent(this.transform.Find("left").Find("left thigh"));
                    leftThigh.transform.localPosition = Vector3.zero;
                    leftThigh.transform.localScale = new Vector3(1, 1, 1);
                    //right leg
                    GameObject rightLeg = armor.transform.Find("rightLeg").gameObject;
                    rightLeg.transform.SetParent(this.transform.Find("right").Find("right leg"));
                    rightLeg.transform.localPosition = Vector3.zero;
                    rightLeg.transform.localScale = new Vector3(1, 1, 1);
                    //left leg
                    GameObject leftLeg = armor.transform.Find("leftLeg").gameObject;
                    leftLeg.transform.SetParent(this.transform.Find("left").Find("left leg"));
                    leftLeg.transform.localPosition = Vector3.zero;
                    leftLeg.transform.localScale = new Vector3(1, 1, 1);

                    rightThigh.SetActive(true);
                    leftThigh.SetActive(true);
                    rightLeg.SetActive(true);
                    leftLeg.SetActive(true);
                    equippedSlots["rightThigh"] = true;
                    equippedSlots["leftThigh"] = true;
                    equippedSlots["rightLeg"] = true;
                    equippedSlots["leftLeg"] = true;
                }
                break;
            case 5:
                if (!equippedSlots["rightLeg"] && !equippedSlots["leftLeg"])
                {
                    //right leg
                    GameObject rightLeg = armor.transform.Find("rightLeg").gameObject;
                    rightLeg.transform.SetParent(this.transform.Find("right").Find("right leg"));
                    rightLeg.transform.localPosition = Vector3.zero;
                    rightLeg.transform.localScale = new Vector3(1, 1, 1);
                    //left leg
                    GameObject leftLeg = armor.transform.Find("leftLeg").gameObject;
                    leftLeg.transform.SetParent(this.transform.Find("left").Find("left leg"));
                    leftLeg.transform.localPosition = Vector3.zero;
                    leftLeg.transform.localScale = new Vector3(1, 1, 1);

                    rightLeg.SetActive(true);
                    leftLeg.SetActive(true);
                    equippedSlots["rightLeg"] = true;
                    equippedSlots["leftLeg"] = true;
                }
                    break;
            default:
                break;
        }
        
    }
}
