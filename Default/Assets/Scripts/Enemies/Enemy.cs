using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent (typeof (Controller2D))]
public class Enemy : MonoBehaviour {

    //_________________________________________________________________________ 
    //declaring variables: 
    //here we're just telling the script we will be using these vairables and what type they are (string, int, float, Vector3, GameObject, etc)
    //we're not actually assigning them any values yet
    //public means (1) other scripts can use them, (2) you can assign them from the Inspector menu in Unity(drag and drop)

    //variable types:
    /* 
     string, float, boolean : basic types
     Vector2 : stores two values (x,y), used for position (think coordinate plane)
     Vector3 : stores three values (x,y,z), used for force vectors (think physics vectors)
     GameObject : a reference to game objects in the Hierarchy Window in Unity (on the left with all the different objects like Main Camera, Player, enemy_pinbol, Obstacles, etc)
     Animator : a gameobject that controls animation, it's the one with the flowchart and connecting different animations for a gameobject
     Script Names : references to scripts we created ourselves (ex. EnemyStats, EnemyHUD, PlayerStats, Controller2D)
         */

    /* note about GameObjects and Components:
       Components are components of GameObjects
       (ex)
       Enemy (GameObject)
        - Sprite (Component) : this is the sprite of enemy
        - Transform (Component) : this contains information such as rotation, scale, and position
        - Box Collider 2D (Component) : this is the green bounding box that detects collision
        - Animator (Component) : controls animation for this GameObject(Enemy) only

        Scripts that we wrote ourselves can also be Components
         */

    EnemyStats stats;       //reference to EnemyStats script (used to assign enemy stats)
    EnemyHUD HUD;           //reference to EnemeyHUD script (used to control enemy HUD (nametag, health bar, etc))

    public GameObject Player;   //reference to Player object (assigned in Inspector)
    PlayerStats playerStats;    //reference to PlayerStats script that is a component of Player

    float size;
    float health;

    public float moveSpeed;
    int directionX = 1;

    float gravity;
    float jumpVelocity;
    float targetVelocityX;
    float velocityXDamper;  //don't need to care about this, this is handled by the Mathf.SmoothDamp later on

    //used for calculate gravity and jump velocity using UAM equations
    public float jumpHeight;
    public float timeToJumpApex;

    //for trigging falling animation
    float timeInAir;
    bool isInAir;
    float startTime;

    Vector3 velocity;   //Vector3 is just another variable type like string and integer, it stores 3 values -> (x,y,z)

    Controller2D controller;    //reference Controller2D script
    Animator anim;              //reference to Animator, the animation controller that has the animation flowchart

    public bool isHopEnabled = true;    //is enemy of hopping type

    bool isMove = true;    //is moving or not

    int executeOnce = 0;            //one way flag
    int executeOnceTimeLimit = 0;

    Vector3 spawnPoint;

    EnemyDropLoot dropLoot;     //reference to drop loot script

    public Slider playerExpSlider;  //reference to player's experience bar

    bool isHurt = false;    //is in the process of getting hurt or not

    public int facingDirectionInitial;  //initial facing direction (1 is right, -1 is left, can be set in Inspector for level design purposes)


    //_________________________________________________________________________ 
    //now our functions
    //Start() : is run ONCE when the game starts, used for initilizing varaibles and grabbing references
    //Update() : runs every frame
    //Hit() : runs when is attacked by player's weapon
    //Flip() : used to flip facing direction based on velocity.x (is velocity.x is positive then face in the 1 direction, if negative then face in the -1 direction)
    //Idle() : used to roll the dice to determine whether enemy should idle or not
    //actuallyDie() : used to run death animation, droploot, and respawn

    //the void in front of the function name means that the function will not be returning any values
    //for example if you want a function to determine whether a person is cute or not

    /* bool determineCuteOrNot(nameOfPerson) {
            if (nameOfPerson == "mip") {
                return true;  
          } else {
                return false;
          }
       }
    */

    //so when we call the determineCuteOrNot function
    //we call it like this
    //determineCuteOrNot("Jeff")
    //and then it will return false
    //which is a boolean, which we put before the function name
    //if the function is not returning anything, put void


    //IEnumerator is another variable type
    //use it for functions that use the function WaitForSeconds()
    //which is basically just a wait function
    //it's used in Idle() because we want it to idle, wait, and then start moving again
    //it's also used in actuallyDie() because we want it to die, wait, respawn



    //Start() is run once when the game starts, used for initilizing variables and references and such
    void Start() {
        //here we actually reference each variable to their components
        //since these five components shares the same GameObject (Enemy) as this script, we can use GetComponent<ComponentName>() function to reference it
        controller = GetComponent<Controller2D>();  //used for collision detection
        anim = GetComponent<Animator>();            //used for controlling animation
        stats = GetComponent<EnemyStats>();         //used to read and write enemy stats
        HUD = GetComponent<EnemyHUD>();             //used to control enemy Head Up Display
        dropLoot = GetComponent<EnemyDropLoot>();   //used to run drop loot functions

        //kinematics equation (phun)
        gravity = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        //set
        health = stats.enemyStats.Vitality.BaseValue * 10;   //BaseValue is the value of Vitality, Vitality is an attribute of object enemyStats, which is located in stats script (which is referenced by GetComponent<EnemyStats>())
        size = stats.size;

        //reset localScale and orientation/facing direction
        transform.localScale = new Vector3(size * facingDirectionInitial, size, size);

        //get player stats
        playerStats = Player.GetComponent<PlayerStats>();   //now it's not just GetComponent, because PlayerStats is a component of Player GameObject, hence the Player. in front of GetComponent

        //set spawn point
        spawnPoint = transform.position;    //spawnPoint is a Vector3 with (x,y,z) values, so does transform.position, which is the position of this GameObject (Enemy)
    }

    void Update() {
        //for initilization and dropping to ground (this block of code is convoluted 102 ~ 119)
        if (executeOnce == 0)
        {
            //move downward until hitting ground
            velocity.y -= 1;
            controller.Move(velocity * Time.deltaTime);
            if (controller.collisions.below)
            {
                executeOnce = 1;    //activate one way flag
            }
        }
        else if (executeOnce == 1) {
            //initialization once hit ground for first time
            isMove = true;
            isInAir = false;
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalk", true);
            executeOnce = 2;
        }

        //set target velocity x
        targetVelocityX = (isMove) ? moveSpeed * directionX : 0;

        /*note: syntax for shorthand if and else statement is
        variable = (condition) ? a : b
        variable = a if condition is true and = b if condition is false*/

        //switch direction when collide horizontally
        //controller is reference to the Controller2D script that is in charge of collision detections
        //collisions is a struc (think it as container) inside the Controller2D script
        //controller.collisions.left is true when colliding on the left
        directionX = (controller.collisions.left) ? 1 : ((controller.collisions.right) ? -1 : directionX);

        //flip facing direction
        if (targetVelocityX != 0)
        {
            Flip(velocity); //call Flip function which is defined below
        }

        //random chance to Idle
        //in words: if on ground and is moving, then call function that might start the Idle process, if on ground and not moving then set y velocity to 0
        if (controller.collisions.below)    //if on ground
        {
            anim.SetBool("isFall", false);                  //cancel falling animation
            if (isMove)                                     //if is moving
            {
                StartCoroutine(Idle(UnityEngine.Random.Range(2, 6)));   //call function that calculates probability to start Idleing
            }
            else
            {
                velocity.y = 0;                             //if on ground and not moving, reset y velocity to 0 (to avoid accumulating gravity vector)
            }
        }

        //calculating and determining whether is falling past original jumping point
        //the instant that enemy is touching ground and y velocity is greater than zero is the instant the enemy is about to jump
        //so we start a timer by setting initial time(startTime) to current time(Time.time)
        if (controller.collisions.below && velocity.y > 0) {
            startTime = Time.time;
        }   //and then if NOT touching ground and new current time(Time.time) - initial time(startTime) is greater than an arbitary number (1f), then change animation from jumping to falling
        if (!controller.collisions.below && (Time.time - startTime) > 1f) {
            anim.SetBool("isFall", true);
        }

        //set walking animation speed in accordance to time spent in air (for hoppers only)
        if (Mathf.Abs(targetVelocityX) > 0 && isHopEnabled)
        {
            anim.SetFloat("moveSpeed", ((timeToJumpApex * 2) / anim.GetCurrentAnimatorClipInfo(0).Length));
        }
        else
        {   
            //if not hoppers then set walking animation speed to move speed
            anim.SetFloat("moveSpeed", moveSpeed);  
        }

        //actual movement
        velocity.x = (health > 0) ? Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXDamper, 0.2f) : 0;
        velocity.y += gravity * Time.deltaTime;

        /*note: syntax for Mathf.SmoothDamp
        a = Mathf.SmoothDamp(a, b, ref c, d)
        where
        a is the current velocity
        b is the final velocity
        c is a variable that we don't need to care about, it is just for SmoothDamp to mess with
        d is the smooth time
        SmoothDamp smoothes the transition from a to b*/

        //now we pass in velocity into the Move() function in Controller2D script
        controller.Move(velocity * Time.deltaTime);
    }

    //when attacked
    void Hit(float damage) {
        if (!isHurt)
        {
            isHurt = true;
            health -= playerStats.playerStats.Power.BaseValue;      //decrease health by player power stats
            HUD.showHealth(health);                                 //update HUD
            anim.SetTrigger("isHurt");                              //trigger hurt animation
            velocity.x -= playerStats.playerStats.Knockback.BaseValue * Mathf.Sign(Player.transform.position.x - gameObject.transform.position.x);   //knockback
            isHurt = false;
        }

        //dying conditions
        if (health <= 0) {
            if (!anim.GetBool("isDie"))
            {
                anim.SetBool("isDie", true);
                
                playerStats.playerStats.Experience.BaseValue += stats.enemyStats.Experience.BaseValue;

                if (playerStats.playerStats.Experience.BaseValue >= playerExpSlider.maxValue) {
                    playerStats.levelUp(playerStats.playerStats.Experience.BaseValue - playerExpSlider.maxValue);
                }

                try
                {
                    playerStats.updateStatWindow(false, "Experience");
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    //flip facing direction
    void Flip(Vector2 input)
    {
        if (Mathf.Sign(velocity.x) == 1)
        {
            transform.localScale = new Vector3(-size, size, size);  //flip sprites
            HUD.flipHUD(-1);                                        //flip HUD to keep it always in the same orientation
        }
        else if (Mathf.Sign(velocity.x) == -1)
        {
            transform.localScale = new Vector3(size, size, size);   //flip sprites
            HUD.flipHUD(1);                                         //flip HUD to keep it always in the same orientation
        }
    }

    //chance to Idle and Idleling script
    IEnumerator Idle(float sec)
    {
        if (UnityEngine.Random.Range(0, (isHopEnabled) ? 5 : 500) < 1)      //chance to idle
        {
            isMove = false;
            anim.SetBool("isWalk", false);
            anim.SetBool("isIdle", true);
            yield return new WaitForSeconds(sec);                           //Idle time
            directionX *= (UnityEngine.Random.Range(0, 2) < 1) ? -1 : 1;    //random chance to face opposite direction
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalk", true);
            isMove = true;
        }
        else {
            velocity.y = (isHopEnabled) ? jumpVelocity : velocity.y;        //if hop enabled, hop if not idle
        }
    }

    IEnumerator actuallyDie(float secRespawnTimer) {
        dropLoot.dropLoot(transform.position);
        transform.position = new Vector3(1000, 1000, 0);
        anim.SetBool("isDie", false);
        yield return new WaitForSeconds(secRespawnTimer);
        health = stats.vitality * 10;
        HUD.showHealth(health);
        transform.position = spawnPoint;
    }
}