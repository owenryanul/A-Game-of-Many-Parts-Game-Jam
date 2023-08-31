using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using TMPro;
using System;

//For the PlayerLogic Readme see Player Readme.txt in Assets/Do not Modify/Essential Prefabs/Player/Readme/Player Readme.txt

public class PlayerLogic : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;
    public bool playerCameraFollowsPlayer = false;

    [Header("Movement")]
    public float topSpeed = 1.0f; //in units/second
    public float acceleration = 1.0f; //in units/second/second
    public float deceleration = -1.0f; //in units/second/second

    private float currentSpeed;
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;

    private Vector2 externalVelocity; //External Velocity that is applied to the player.
    private bool overridePlayerMovementVelocity; //Whether or not external velocity overrides the player's movement or is added instead.

    [Header("Aiming")]
    public float maxReticleDistanceFromPlayer;
    private Vector3 aimDirection;
    private bool usingMouse; //is the player using the mouse to aim

    [Header("Ammo")]
    public List<Ammo> carriedAmmo; //The current selection of ammo types carried by the player
    public Ammo defaultAmmo; //The default ammo type used if carriedAmmo is empty
    private int currentAmmoIndex; 
    private bool changingAmmo; //true if the player is currently swapping ammo types

    [Header("Fire")]
    private GameObject currentAmmoBehaviourPrefab; //The prefab containing the ammobehaviour script containing all the listeners for the currently selected ammo type.
    private bool firePressedOnThisAmmo; //Flag that ensures that OnRelease won't trigger if the user changed ammo types in between pressing and releasing the fire button

    [Header("Activate")]
    public List<OnActivateListener> allOnActivateListeners; //All subscribed listeners when the player uses the activate control.

    [Header("Interact")]
    private Interactable currentInteractable; //The current target for when the player hits the interact control.

    [Header("Dodge Roll")]
    public float dodgeRollVelocity;
    private bool isDodging;
    private Vector3 dodgeRollDirection;

    [Header("Hp")]
    private int currentHp;
    private List<OnDeathListener> allOnDeathListeners;
    private bool dying;

    [Header("Statuses")]
    private List<Status> allStatuses; //A list of all Status's applied to the player. See the status class below for more info.

    [Header("Paused")]
    public bool paused = false;

    [Header("Audio")]
    public AudioClip painSound;
    public AudioClip rollSound;
    public AudioClip deathSound;

    private void OnEnable()
    {
        allOnActivateListeners = new List<OnActivateListener>();
        allOnDeathListeners = new List<OnDeathListener>();
        allStatuses = new List<Status>();
        currentHp = 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        movementDirection = Vector2.zero;
        currentSpeed = 0.0f;
        aimDirection = Vector3.right;
        usingMouse = false;
        changingAmmo = false;
        firePressedOnThisAmmo = false;
        GameObject.FindGameObjectWithTag("base_hpIndicator").GetComponentInChildren<TextMeshProUGUI>().text = "" + this.currentHp;
        dying = false;
        changeAmmo(); //Run the ammo change logic to setup the player's intial ammo-type.
    }

    // Update is called once per frame
    void Update()
    {
        if (!dying && !paused) 
        {
            movementLogic();
            if(playerCameraFollowsPlayer)
            {
                playerCamera.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, playerCamera.transform.position.z);
            }
            aimingLogic();
        }

        if(paused)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player collides with an object tagged as an interactable, set that object as the currentInteractable.
        if (collision.tag == "base_interactable")
        {
            this.setCurrentInteractable(collision.gameObject.GetComponent<Interactable>());
        }

        //Any other logic for player collisions should be handled in the logic for the other object in the collision.
        //E.g. For enemeis dealing damage on touch, put the damage logic in their OnTriggerEnter2D
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the player stops colliding with the currentInteractable, set the current interactable to null.
        if (collision.tag == "base_interactable" && collision.gameObject.GetComponent<Interactable>() == this.getCurrentInteractable())
        {
           this.setCurrentInteractable(null);
        }

        //Any other logic for player collisions should be handled in the logic for the other object in the collision.
        //E.g. For enemeis dealing damage on touch, put the damage logic in their OnTriggerEnter2D
    }

    //===========================[Movement]======================================
    private void movementLogic()
    {
        //movementDirection will be set in OnMovementKeysHit()
        if (isDodging)
        {
            dodgeMovementLogic();
        }
        else
        {
            updateCurrentSpeed();

            if (movementDirection != Vector2.zero)
            {
                //Move the player in the current direction times their current speed.
                this.gameObject.GetComponent<Rigidbody2D>().velocity = movementDirection.normalized * currentSpeed;
            }
            else
            {
                //Move the player in the last direction they were moving times their current speed.
                this.gameObject.GetComponent<Rigidbody2D>().velocity = lastMovementDirection.normalized * currentSpeed;
            }
        }

        applyExternalVelocity();

        //if the character was moving right, make their sprite face right, if they were moving left make their sprite face left, otherwise leave their facing as is.
        if(lastMovementDirection.x < 0)
        {
            setVisualFacing(true);
        }
        else if (lastMovementDirection.x > 0)
        {
            setVisualFacing(false);
        }
    }

    //Determines player speed based on how long they've been moving. They accelerate up to max speed based on acceleration while moving
    // and deccelerate over time while not moving. This is intended to give the player a sense of momentum.
    private void updateCurrentSpeed()
    {
        if (movementDirection == Vector2.zero)
        {
            currentSpeed += (deceleration * Time.deltaTime); //reduce speed as they stand still, so they don't lose all speed the second they let go of the movement keys.
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
                this.gameObject.GetComponent<Animator>().SetBool("Running", false);
            }
        }
        else
        {
            currentSpeed += (acceleration * Time.deltaTime); // acceleration is per second, so multiple by deltatime.
            if (currentSpeed > this.topSpeed)
            {
                currentSpeed = topSpeed;
            }
            this.gameObject.GetComponent<Animator>().SetBool("Running", true);
        }
    }

    /* Applies the externalVelocity vector to the player character's velocity.
     * Set overridePlayerMovementVelocity to true to replaces the player's current velocity with the external velocity.
     * Set overridePlayerMovementVelocity to false to add external velocity to the player's current velocity. */
    private void applyExternalVelocity()
    {
        if(overridePlayerMovementVelocity)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = externalVelocity;
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity += externalVelocity;
        }
    }

    //Modifies the player's movement direction based on the movement controls' input.
    public void OnMovementKeysHit(InputAction.CallbackContext context)
    {
        this.movementDirection = context.ReadValue<Vector2>().normalized;
        if(this.movementDirection != Vector2.zero)
        {
            this.lastMovementDirection = this.movementDirection;
        }
    }

    /* Modifies the facing of the player character's spites to make it look left or right. 
     * isFlipped = true for facing left.
     * isFlipped = false for facing right.
     */

    private void setVisualFacing(bool isFlipped)
    {
        foreach (SpriteRenderer aSprite in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            aSprite.flipX = isFlipped;
        }

        GameObject bowSprite = this.gameObject.transform.Find("Sprite").Find("Bow").gameObject;
        Vector3 bowPos = bowSprite.transform.localPosition;
        if (isFlipped)
        {
            bowSprite.transform.localPosition = new Vector3(-Mathf.Abs(bowPos.x), bowPos.y, bowPos.z);
            bowSprite.transform.localEulerAngles = new Vector3(0, 0, 16.525f);
            this.gameObject.GetComponent<Animator>().SetBool("Mirrored", true);
        }
        else
        {
            bowSprite.transform.localPosition = new Vector3(Mathf.Abs(bowPos.x), bowPos.y, bowPos.z);
            bowSprite.transform.localEulerAngles = new Vector3(0, 0, -16.525f);
            this.gameObject.GetComponent<Animator>().SetBool("Mirrored", false);
        }
    }

    //Returns the current direction the player character is trying to move based on player inputs.
    public Vector2 getPlayerMovementDirection()
    {
        return this.movementDirection;
    }

    //Sets the External Velocity vector that will be applied to the player every update after player movement is calculated.
    //If overridePlayerMovementVelocity is set to true, then external velocity will replcae player movement instead of being applied after it.
    public void setExternalVelocity(Vector2 velocityIn)
    {
        externalVelocity = velocityIn;
    }

    //Adds velocityIn to the current value of the External Velocity vector that will be applied to the player every update after player movement is calculated.
    //If overridePlayerMovementVelocity is set to true, then external velocity will replcae player movement instead of being applied after it.
    public void addExternalVelocity(Vector2 velocityIn)
    {
        externalVelocity += velocityIn;
    }

    //Returns the current value of ExternalVelocity.
    public Vector2 getExternalVelocity()
    {
        return this.externalVelocity;
    }

    //Returns true if external velocity is set to replace the player's input based movement.
    public bool getIsExternalVelocityOverridingPlayerMovement()
    {
        return this.overridePlayerMovementVelocity;
    }

    /* Sets whether or not External Velocity will replace player movement or be added to it.
     * True for replace, false for added to. Defaults to false.*/
    public void setIsExternalVelocityOverridingPlayerMovement(bool doOverridePlayerMovementVelocity)
    {
        this.overridePlayerMovementVelocity = doOverridePlayerMovementVelocity;
    }

    //===========================[End of Movement]======================================

    //===========================[Aiming]======================================

    //Logic for determining where to place the aiming Reticle.
    private void aimingLogic()
    {
        //AimDirection is set in OnAimListener(), unless the player is using a mouse to aim, in which case it is set here.
        //Whether or not the player is aiming with a mouse is set in OnAimListener().
        if (usingMouse)
        {
            updateAimFromMousePosition(Mouse.current.position.ReadValue());
        }

        if (aimDirection.magnitude < 0.1f)
        {
            this.transform.Find("Reticle").GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (usingMouse)
        {
            if (aimDirection.magnitude > maxReticleDistanceFromPlayer)
            {
                this.transform.Find("Reticle").GetComponent<SpriteRenderer>().enabled = true;
                this.transform.Find("Reticle").localPosition = aimDirection.normalized * maxReticleDistanceFromPlayer;
            }
            else
            {
                this.transform.Find("Reticle").GetComponent<SpriteRenderer>().enabled = true;
                this.transform.Find("Reticle").localPosition = aimDirection;
            }
        }
        else
        {
            if (aimDirection.magnitude < maxReticleDistanceFromPlayer)
            {
                this.transform.Find("Reticle").GetComponent<SpriteRenderer>().enabled = true;
                this.transform.Find("Reticle").localPosition = aimDirection * maxReticleDistanceFromPlayer;
            }
            else
            {   
                this.transform.Find("Reticle").GetComponent<SpriteRenderer>().enabled = true;
                this.transform.Find("Reticle").localPosition = aimDirection.normalized * maxReticleDistanceFromPlayer;
            }
        }        
    }

    public void OnAimListener(InputAction.CallbackContext context)
    {
        if (!dying && !paused)
        {
            if (context.control.name == "position")
            {
                usingMouse = true;             
                //If usingMouse, then the aimDirection will be set in aimingLogic()
            }
            else if (context.control.name == "rightStick")
            {
                usingMouse = false;
                aimDirection = context.ReadValue<Vector2>();
            }
        }
    }

    //Called when the player moves so that the aim position.
    //Also called from movementLogic if isUsingFixedCamera is true.
    //Updates the position the player is aiming at based on their position and posIn.
    private void updateAimFromMousePosition(Vector2 posIn)
    {      
        if (playerCamera != null)
        {
            Vector3 mousePosition = playerCamera.ScreenToWorldPoint(posIn);
            mousePosition.z = 0;
            aimDirection = (mousePosition - this.gameObject.transform.position);
        }
    }

    //Returns the direction the player is currently aiming in.
    public Vector3 getAimDirection()
    {
        return this.aimDirection;
    }

    //===========================[End of Aiming]======================================


    //===========================[Fire]===============================================

    public void OnFirePressed(InputAction.CallbackContext context)
    {
        if (!dying && !paused)
        {
            if (context.phase == InputActionPhase.Performed && !this.changingAmmo && !this.isDodging)
            {
                PressInteraction pressInteraction = (PressInteraction)context.interaction;
                if (pressInteraction.behavior == PressBehavior.PressOnly)
                {
                    //Call on fire pressed for the current ammo behaviour
                    currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnFirePressd(this);
                    this.firePressedOnThisAmmo = true;
                }
                else if (pressInteraction.behavior == PressBehavior.ReleaseOnly && this.firePressedOnThisAmmo)
                {
                    //Call on fire released for the current ammo behaviour
                    currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnFireReleased(this);
                    this.firePressedOnThisAmmo = false;
                }
            }
        }
    }

    //===========================[End of Fire]========================================

    //===========================[Ammo]===============================================

    //Select's the next ammo type the player is carrying.
    public void OnNextAmmoPressed(InputAction.CallbackContext context)
    {
        if (!dying && !paused)
        {
            if (context.phase == InputActionPhase.Performed && !changingAmmo)
            {
                currentAmmoIndex++;
                if (currentAmmoIndex >= carriedAmmo.Count)
                {
                    currentAmmoIndex = 0;
                }

                changingAmmo = true;
                GameObject.FindGameObjectWithTag("base_ammoIndicator").GetComponent<Animator>().SetTrigger("Next");
                changeAmmo();
            }
        }   
    }

    //Select's the previous ammo type the player is carrying.
    public void OnPreviousAmmoPressed(InputAction.CallbackContext context)
    {
        if (!dying && !paused)
        {
            if (context.phase == InputActionPhase.Performed && !changingAmmo)
            {
                currentAmmoIndex--;
                if (currentAmmoIndex < 0)
                {
                    currentAmmoIndex = carriedAmmo.Count - 1;
                }

                changingAmmo = true;
                GameObject.FindGameObjectWithTag("base_ammoIndicator").GetComponent<Animator>().SetTrigger("Previous");
                changeAmmo();
            }
        }
    }

    //Performs all the necessary tasks to switch to the current ammo type.
    //Calls Unequip (and cancel, if necessary) listeners on the pre-existing ammo behaviour prefab, then destroys that prefab 
    //and spawns in the ammo behaviour prefab for the current ammo type, then calls the onequip listener for it.
    private void changeAmmo()
    {
        if (currentAmmoBehaviourPrefab != null)
        {
            if(this.firePressedOnThisAmmo) //if the player has pressed but not released fire, then call OnFireCancelled()
            {
                currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnFireCancelled(this);
            }
            currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnUnequip(this);
            Destroy(currentAmmoBehaviourPrefab);
        }

        firePressedOnThisAmmo = false;
        currentAmmoBehaviourPrefab = Instantiate(this.getAmmoRelativeToCurrent(0).ammoBehaviourPrefab, this.gameObject.transform);
        currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnEquip(this);
    }


    //Return the Ammo object that is offset entries away from currentlySelectedAmmoType in CarriedAmmo
    //Recurivily loops through CarriedAmmo if the provided offset would be outside the bounds of the list.
    //e.g. getAmmoRelativeToCurrent(0) would return the currently selected ammo.
    //     getAmmoRelativeToCurrent(1) would return the next ammo in the list.
    //     getAmmoRelativeToCurrent(-2) would return the ammo 2 entries earlier in the list.
    public Ammo getAmmoRelativeToCurrent(int offset)
    {
        //Handle edges cases where the list is too small
        if(this.carriedAmmo.Count < 1)
        {
            return defaultAmmo;
        }
        else if(this.carriedAmmo.Count == 1)
        {
            return this.carriedAmmo[0];
        }


        if(offset + currentAmmoIndex >= this.carriedAmmo.Count)
        {
            return recursiveCountUpFromZero((offset + currentAmmoIndex) - this.carriedAmmo.Count);
        }

        if (offset + currentAmmoIndex < 0)
        {
            return recursiveCountDownFromTop(offset + currentAmmoIndex);
        }

        return this.carriedAmmo[offset + currentAmmoIndex];
    }

    //Recursive method used to loop getAmmoRelativeToCurrent() forwards through the list when a positive offset exceeds carriedAmmo's size.
    private Ammo recursiveCountUpFromZero(int i)
    {
        if (i  >= this.carriedAmmo.Count)
        {
            return recursiveCountUpFromZero((i - this.carriedAmmo.Count));
        }
        else
        {
            return this.carriedAmmo[i];
        }
    }

    //Recursive method used to loop getAmmoRelativeToCurrent() backwards through the list when a negative offset is given that would go below zero.
    private Ammo recursiveCountDownFromTop(int i)
    {
        if (i + this.carriedAmmo.Count < 0)
        {
            return recursiveCountDownFromTop(this.carriedAmmo.Count + (i));
        }
        else
        {
            return this.carriedAmmo[this.carriedAmmo.Count + i];
        }
    }

    //Returns the ammo type carried by the player that makes the passed Name.
    public Ammo getAmmoFromName(string name)
    {
        foreach(Ammo aAmmo in carriedAmmo)
        {
            if(aAmmo.name == name)
            {
                return aAmmo;
            }
        }
        return null;
    }

    //Adds the passed Ammo to the player's carried ammo.
    public void addAmmo(Ammo ammoToAdd)
    {
        //if the ammo type is already carried, increase the quanity
        foreach(Ammo aAmmo in this.carriedAmmo)
        {
            if(aAmmo.name == ammoToAdd.name)
            {
                aAmmo.quantity += ammoToAdd.quantity;
                GameObject.FindGameObjectWithTag("base_ammoIndicator").GetComponent<AmmoIndicatorLogic>().setAmmoIndicator();
                return;
            }
        }

        //If the ammo type is not already carried, add the ammo as a new type
        this.carriedAmmo.Add(ammoToAdd);
        GameObject.FindGameObjectWithTag("base_ammoIndicator").GetComponent<AmmoIndicatorLogic>().setAmmoIndicator();
        //If carried ammo was empty before this ammo was picked up, update the logic to equip the new ammo type.
        if(this.carriedAmmo.Count == 1)
        {
            currentAmmoIndex = 0;
            changeAmmo();
        }
    }

    //Find's the matching Ammo that is carried by the player and add "amount" to it's quantity. If the new quantity is 0 or less, remove the ammo from the list.
    //Returns true if ammoModified successfully, else returns false.
    public bool modifyAmmoAmount(Ammo ammoToModify, int amount)
    {
        int indexOfAmmo = -1;
        for(int i = 0; i < carriedAmmo.Count; i++)
        {
            if(carriedAmmo[i].name == ammoToModify.name)
            {
                indexOfAmmo = i;
                break;
            }
        }

        if(indexOfAmmo < 0)
        {
            Debug.LogWarning("Warning: Could not find Ammo with name " + ammoToModify.name + " in the list of carried ammo.");
            return false;
        }
        else
        {
            carriedAmmo[indexOfAmmo].quantity += amount;
            if(carriedAmmo[indexOfAmmo].quantity < 1)
            {
                carriedAmmo.RemoveAt(indexOfAmmo);
                if (currentAmmoIndex >= indexOfAmmo)
                {
                    currentAmmoIndex--;
                    if (currentAmmoIndex < 0)
                    {
                        currentAmmoIndex = 0;
                    }
                }
                changeAmmo();
            }
            GameObject.FindGameObjectWithTag("base_ammoIndicator").GetComponent<AmmoIndicatorLogic>().setAmmoIndicator();
        }
        return true;
    }

    //Marks the ammo changing process as complete and that it is safe for normal ammo interaction logic to proceed.
    //Called from AmmoIndicatorLogic.setAmmoIndicator()
    public void isDoneChangingAmmo()
    {
        this.changingAmmo = false;
    }

    //===========================[End of Ammo]========================================

    //===========================[Activate]===============================================

    //Calls the OnActivatePressed/OnActivateReleased listeners for all currently subscribed OnActivateListeners.
    public void OnActivatePressed(InputAction.CallbackContext context)
    {
        if (!dying && !paused)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PressInteraction pressInteraction = (PressInteraction)context.interaction;
                if (pressInteraction.behavior == PressBehavior.PressOnly)
                {
                    foreach (OnActivateListener aListener in allOnActivateListeners)
                    {
                        aListener.OnActivatePressed();
                    }
                }
                else if (pressInteraction.behavior == PressBehavior.ReleaseOnly)
                {
                    foreach (OnActivateListener aListener in allOnActivateListeners)
                    {
                        aListener.OnActivateReleased();
                    }
                }
            }
        }
    }

    //Subscribe's the passed OnActivateListener to listen for when the player presses or releases the Activate key.
    public void addOnActivateListener(OnActivateListener listenerIn)
    {
        this.allOnActivateListeners.Add(listenerIn);
    }

    //Unsubscribe's the passed OnActivateListener from listening for when the player presses or releases the Activate key.
    public void removeOnActivateListener(OnActivateListener listenerIn)
    {
        this.allOnActivateListeners.Remove(listenerIn);
    }

    //Returns true if the passed OnActivateListener is subscribed.
    public bool isAnOnActivateListener(OnActivateListener listenerIn)
    {
        return allOnActivateListeners.Contains(listenerIn);
    }
    //===========================[End of Activate]========================================

    //===========================[Interact]========================================

    //When the player hit sthe Interact key, calls OnInteracted() for the interactable that the player is currently colliding with.
    public void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (!dying && !paused)
        {
            if (context.phase == InputActionPhase.Performed && this.getCurrentInteractable() != null)
            {
                this.currentInteractable.onInteracted();
            }
        }
    }

    //Returns the Interactable the player is currently colliding with.
    public Interactable getCurrentInteractable()
    {
        return this.currentInteractable;
    }

    //Sets the current interactable to the passed interactable. Also calls the OnBecomeCurrentInteractable() on the passed interactable and onNoLongerCurrentInteractable()
    // on the previous interactable.
    // This method accepts null for setting the current interactable to none.
    //Note for manually calling this: This method is automatically called in this.OnTriggerEnter2D and this.OnTriggerExit2D, bear this in mind when manually setting the
    //current interactable.
    public void setCurrentInteractable(Interactable interactableIn)
    {
        if(interactableIn != null)
        {
            interactableIn.onBecomeCurrentInteractable();
        }

        if (this.currentInteractable != null)
        {
            this.currentInteractable.onNoLongerCurrentInteractable();
        }

        this.currentInteractable = interactableIn;
    }

    //===========================[End of Interact]========================================

    //===========================[Dodge]========================================

    //Execute the dodge roll in the current/last movement direction when the player hits the dodge roll key.
    public void OnDodgePressed(InputAction.CallbackContext context)
    {
        if (!dying && !paused)
        {
            if (context.phase == InputActionPhase.Performed && !this.isDodging)
            {
                this.setIsDodging(true);
                this.dodgeRollDirection = this.lastMovementDirection;
                this.gameObject.GetComponent<Animator>().SetTrigger("Roll");
                this.gameObject.GetComponent<AudioSource>().PlayOneShot(rollSound);
            }
        }
    }

    //Marks the player as no longer dodging.
    //Also forces a re-check of the colliders in case the player is inside of a hitbox when the dodge roll ends. This is to cover situations like the player rolling
    //into an enemy, then finishing the roll inside of the enemy and not taking damage because OnTriggerEnter only triggered when they were rolling.
    public void OnDodgeAnimationDone()
    {
        this.setIsDodging(false);
        
        Collider2D playerCollide = this.gameObject.GetComponent<Collider2D>();
        //Physics.SyncTransforms();
        playerCollide.enabled = false;
        playerCollide.enabled = true;
    }

    public bool getIsDodging()
    {
        return this.isDodging;
    }

    public void setIsDodging(bool inDodging)
    {
        this.isDodging = inDodging;
        
        //if the player starts dodging and had pressed fire but hadn't released it yet, then call OnFireCancel() for the current ammo behaviour.
        //This is because dodging intrupts any instance of a player holding the fire button on an ammo type.
        if(isDodging && this.firePressedOnThisAmmo)
        {
            this.currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnFireCancelled(this);
            this.firePressedOnThisAmmo = false;
        }
    }

    //Set's the player's velocity to the velocity of the dodge roll.
    private void dodgeMovementLogic()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = dodgeRollDirection * dodgeRollVelocity;
    }

    //===========================[End of Dodge]========================================

    //===========================[Hp]========================================

    //Set the player's current hp, set trigger hurt animation to true if you want to play the hurt animation when doing this.
    public void setHp(int HPin, bool triggerHurtAnimation = false)
    {
        this.currentHp = HPin;
        GameObject.FindGameObjectWithTag("base_hpIndicator").GetComponentInChildren<TextMeshProUGUI>().text = "" + this.currentHp;
        if(this.currentHp <= 0)
        {
            this.die();
        }
        else if(triggerHurtAnimation)
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("Hurt");
        }
    }

    //Add the passed HP to the player's current hp, set trigger hurt animation to true if you want to play the hurt animation when doing this.
    //Pass a negative value to reduce player hp.
    public void addHp(int HPtoAdd, bool triggerHurtAnimation = false)
    {
        this.currentHp += HPtoAdd;
        GameObject.FindGameObjectWithTag("base_hpIndicator").GetComponentInChildren<TextMeshProUGUI>().text = "" + this.currentHp;
        if (this.currentHp <= 0)
        {
            this.die();
        }
        else if (triggerHurtAnimation)
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("Hurt");
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(painSound);
        }
    }
    
    //Returns the player's current hp.
    public int getHP()
    {
        return this.currentHp;
    }

    //Subscribe an OnDeathListener that triggers when the player dies.
    public void addOnDeathListener(OnDeathListener listenerIn)
    {
        this.allOnDeathListeners.Add(listenerIn);
    }

    //Unsubscribe an OnDeathListener from triggering when the player dies.
    public void removeOnDeathListener(OnDeathListener listenerToRemove)
    {
        this.allOnDeathListeners.Remove(listenerToRemove);
    }

    //Kills the player character.
    private void die()
    {
        if (!this.dying)
        {
            this.dying = true;
            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.gameObject.GetComponent<Animator>().SetBool("Die", true);
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(deathSound);
            //OnDeathListeners are triggered at the end of the Death Animation.
        }
    }

    //Respawn the player where they are with 1 hp.
    public void respawn()
    {
        this.dying = false;
        this.currentHp = 1;
        this.gameObject.GetComponent<Animator>().SetBool("Die", false);
    }

    //Calls OnPlayerDies for all subscribed OnDeathListeners.
    //Called from an animation event at the end of the player's death animation.
    public void OnDeathAnimationFinished()
    {
        foreach(OnDeathListener aListener in allOnDeathListeners)
        {
            aListener.OnPlayerDies(this);
        }
    }

    //===========================[End of Hp]========================================

    //===========================[Status]========================================


    //Status, a class made up of 2 strings, used to attach additional data to the player character if needed.
    //Statuses can be used by custom scripts as needed.
    //e.g. attach a wet condition with a value of 10.
    [System.Serializable]
    public class Status
    {
        public string id;
        public string data;

        public Status(string idIn, string dataIn)
        {
            id = idIn;
            data = dataIn;
        }
    }

    //Adds a new status to the player's list of Statuses.
    public void addStatus(string newId, string newData)
    {
        foreach (Status aStatus in allStatuses)
        {
            if (aStatus.id == newId)
            {
                Debug.LogWarning("addStatus has replaced pre-existing Status with id " + newId);
                aStatus.data = newData;
                return;
            }
        }
        allStatuses.Add(new Status(newId, newData));
    }

    //Gets the data field of the Status in the player's list of statuses matching the passed ID.
    //Returns Null if the player does not have a status with the matching ID.
    public string getStatusData(string id)
    {
        foreach(Status aStatus in allStatuses)
        {
            if(aStatus.id == id)
            {
                return aStatus.data;
            }
        }
        return null;
    }

    //Sets the data field of the Status in the player's list of statuses matching the passed ID.
    //Returns true if successful, returns false if it could not find a status matching the ID.
    public bool modifyStatus(string id, string newData)
    {
        foreach (Status aStatus in allStatuses)
        {
            if (aStatus.id == id)
            {
                aStatus.data = newData;
                return true;
            }
        }
        return false;
    }

    //Removes the Status in the player's list of statuses matching the passed ID.
    public void removeStatus(string idToRemove)
    {
        Status statusToRemove = null;
        foreach (Status aStatus in allStatuses)
        {
            if (aStatus.id == idToRemove)
            {
                statusToRemove = aStatus;
                break;
            }
        }

        if(statusToRemove != null)
        {
            allStatuses.Remove(statusToRemove);
        }
    }

    //===========================[End of Status]========================================

    //===========================[Misc]========================================


    //===========================[End of Misc]========================================


}
