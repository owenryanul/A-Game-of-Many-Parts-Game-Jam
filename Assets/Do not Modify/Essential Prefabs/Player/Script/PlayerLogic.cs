using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using TMPro;
using System;

public class PlayerLogic : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;
    public bool playerCameraFollowsPlayer = false;

    [Header("Movement")]
    public float topSpeed = 1.0f; //in units/second
    public float acceleration = 1.0f; //in units/second/second
    public float deceleration = -1.0f; //units lost per units/second/second

    private float currentSpeed;
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;

    private Vector2 externalVelocity;
    private bool overridePlayerMovementVelocity;

    [Header("Aiming")]
    public float maxReticleDistanceFromPlayer;
    private Vector3 aimDirection;
    private bool usingMouse;

    [Header("Ammo")]
    public List<Ammo> carriedAmmo;
    public Ammo defaultAmmo;
    private int currentAmmoIndex;
    private bool changingAmmo;

    [Header("Fire")]
    private GameObject currentAmmoBehaviourPrefab;
    private bool firePressedOnThisAmmo; //Flag that ensures that OnRelease won't trigger if the user changed ammo types in between pressing and releasing the fire button

    [Header("Activate")]
    public List<OnActivateListener> allOnActivateListeners;

    [Header("Interact")]
    private Interactable currentInteractable;

    [Header("Dodge Roll")]
    public float dodgeRollVelocity;
    private bool isDodging;
    private Vector3 dodgeRollDirection;

    [Header("Hp")]
    private int currentHp;
    private List<OnDeathListener> allOnDeathListeners;
    private bool dying;

    [Header("Statuses")]
    private List<Status> allStatuses;

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
        changeAmmo();
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
        if (collision.tag == "base_interactable")
        {
            this.setCurrentInteractable(collision.gameObject.GetComponent<Interactable>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_interactable" && collision.gameObject.GetComponent<Interactable>() == this.getCurrentInteractable())
        {
           this.setCurrentInteractable(null);
        }
    }

    //===========================[Movement]======================================
    private void movementLogic()
    {
        //movementDirection will be set in OnMovementKeysChanged()
        if (isDodging)
        {
            dodgeMovementLogic();
        }
        else
        {
            updateCurrentSpeed();

            if (movementDirection != Vector2.zero)
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = movementDirection.normalized * currentSpeed;
            }
            else
            {
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

    public void OnMovementKeysHit(InputAction.CallbackContext context)
    {
        this.movementDirection = context.ReadValue<Vector2>().normalized;
        if(this.movementDirection != Vector2.zero)
        {
            this.lastMovementDirection = this.movementDirection;
        }
    }

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

    public bool getIsExternalVelocityOverridingPlayerMovement()
    {
        return this.overridePlayerMovementVelocity;
    }

    public void setIsExternalVelocityOverridingPlayerMovement(bool doOverridePlayerMovementVelocity)
    {
        this.overridePlayerMovementVelocity = doOverridePlayerMovementVelocity;
    }

    //===========================[End of Movement]======================================

    //===========================[Aiming]======================================

    private void aimingLogic()
    {
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
    //Updates the position the player is aiming at based on their position and posIn
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
                    currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnFirePressd(this);
                    this.firePressedOnThisAmmo = true;
                }
                else if (pressInteraction.behavior == PressBehavior.ReleaseOnly && this.firePressedOnThisAmmo)
                {
                    currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnFireReleased(this);
                    this.firePressedOnThisAmmo = false;
                }
            }
        }
    }

    //===========================[End of Fire]========================================

    //===========================[Ammo]===============================================

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

    //Marks the ammo changing process as complete and normal ammo interaction logic proceed.
    //Called from AmmoIndicatorLogic.setAmmoIndicator()
    public void isDoneChangingAmmo()
    {
        this.changingAmmo = false;
    }

    //===========================[End of Ammo]========================================

    //===========================[Activate]===============================================
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

    public void addOnActivateListener(OnActivateListener listenerIn)
    {
        this.allOnActivateListeners.Add(listenerIn);
    }

    public void removeOnActivateListener(OnActivateListener listenerIn)
    {
        this.allOnActivateListeners.Remove(listenerIn);
    }

    //TODO: Test this.
    public bool isAnOnActivateListener(OnActivateListener listenerIn)
    {
        return allOnActivateListeners.Contains(listenerIn);
    }
    //===========================[End of Activate]========================================

    //===========================[Interact]========================================

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

    public Interactable getCurrentInteractable()
    {
        return this.currentInteractable;
    }

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
        
        //if the player starts dodging and had pressed fire but hadn't released it yet, then call OnCancel().
        //This is because dodging intrupts any instance of a player holding the fire button on an ammo type.
        if(isDodging && this.firePressedOnThisAmmo)
        {
            this.currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnFireCancelled(this);
            this.firePressedOnThisAmmo = false;
        }
    }

    private void dodgeMovementLogic()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = dodgeRollDirection * dodgeRollVelocity;
    }

    //===========================[End of Dodge]========================================

    //===========================[Hp]========================================

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

    public int getHP()
    {
        return this.currentHp;
    }

    public void addOnDeathListener(OnDeathListener listenerIn)
    {
        this.allOnDeathListeners.Add(listenerIn);
    }

    public void removeOnDeathListener(OnDeathListener listenerToRemove)
    {
        this.allOnDeathListeners.Remove(listenerToRemove);
    }

    private void die()
    {
        if (!this.dying)
        {
            this.dying = true;
            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.gameObject.GetComponent<Animator>().SetBool("Die", true);
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(deathSound);
        }
    }

    public void respawn()
    {
        this.dying = false;
        this.currentHp = 1;
        this.gameObject.GetComponent<Animator>().SetBool("Die", false);
    }

    public void OnDeathAnimationFinished()
    {
        foreach(OnDeathListener aListener in allOnDeathListeners)
        {
            aListener.OnPlayerDies(this);
        }
    }

    //===========================[End of Hp]========================================

    //===========================[Status]========================================

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
