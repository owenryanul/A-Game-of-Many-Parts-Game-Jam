using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player_Logic : MonoBehaviour
{
    [Header("Movement")]
    public float topSpeed = 1.0f; //in units/second
    public float acceleration = 1.0f; //in units/second/second
    public float deceleration = -1.0f; //units lost per units/second/second

    private float currentSpeed;
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;

    public Vector2 externalVelocity;
    private bool overridePlayerMovementVelocity;

    [Header("Aiming")]
    public Vector3 aimDirection;
    public float maxReticleDistanceFromPlayer;
    private bool usingMouse;

    [Header("Ammo")]
    public int currentAmmoIndex;
    public List<Ammo> carriedAmmo;
    public Ammo defaultAmmo;
    public bool changingAmmo;

    private GameObject currentAmmoBehaviourPrefab;

    // Start is called before the first frame update
    void Start()
    {
        movementDirection = Vector2.zero;
        currentSpeed = 0.0f;
        aimDirection = Vector3.right;
        usingMouse = false;
        changingAmmo = false;
        changeAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        movementLogic();
        aimingLogic();
    }

    //===========================[Movement]======================================
    private void movementLogic()
    {
        //movementDirection will be set in OnMovementKeysChanged()

        updateCurrentSpeed();

        if (movementDirection != Vector2.zero)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = movementDirection.normalized * currentSpeed;

            if (usingMouse)
            {
                updateAimFromMousePosition(Mouse.current.position.ReadValue());
            }
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = lastMovementDirection.normalized * currentSpeed;
        }

        applyExternalVelocity();
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
            }
        }
        else
        {
            currentSpeed += (acceleration * Time.deltaTime); // acceleration is per second, so multiple by deltatime.
            if (currentSpeed > this.topSpeed)
            {
                currentSpeed = topSpeed;
            }
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
        //Debug.Log("movement direction changed, is now: " + this.movementDirection);
    }

    public void setExternalVelocity(Vector2 velocityIn, bool doOverridePlayerMovement = false)
    {
        overridePlayerMovementVelocity = doOverridePlayerMovement;
        externalVelocity = velocityIn;
    }

    public void addExternalVelocity(Vector2 velocityIn, bool doOverridePlayerMovement = false)
    {
        overridePlayerMovementVelocity = doOverridePlayerMovement;
        externalVelocity += velocityIn;
    }

    //===========================[End of Movement]======================================

    //===========================[Aiming]======================================

    private void aimingLogic()
    {
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

    public void onAimListener(InputAction.CallbackContext context)
    {
        //context.ReadValue<Vector2>().normalized;
        if (context.control.name == "position")
        {
            usingMouse = true;
            //this.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            updateAimFromMousePosition(context.ReadValue<Vector2>());
        }
        else if (context.control.name == "rightStick")
        {
            usingMouse = false;
            aimDirection = context.ReadValue<Vector2>();
            //this.targetPosition = this.gameObject.transform.position + cursorMoveDirection;// * cursorSpeedOnController;
            //Debug.Log("Updated Controller Position " + cursorMoveDirection + " : " + this.targetPosition);
        }
    }

    //Called when the player moves so that the aim position is updated when the player moves even if the player doesn't move the mouse.
    //Note: This won't be necessary if the camera is locked to the player's position.
    private void updateAimFromMousePosition(Vector2 posIn)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(posIn);
        mousePosition.z = 0;
        aimDirection = (mousePosition - this.gameObject.transform.position);
    }

    public Vector3 getAimDirection()
    {
        return this.aimDirection.normalized;
    }

    //===========================[End of Aiming]======================================


    //===========================[Fire]===============================================

    public void OnFirePressed(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed && !changingAmmo)
        {
            PressInteraction pressInteraction = (PressInteraction)context.interaction;
            if(pressInteraction.behavior == PressBehavior.PressOnly)
            {
                currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnPress(this);             
            }
            else if(pressInteraction.behavior == PressBehavior.ReleaseOnly)
            {
                currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnRelease(this);
            }
        }
    }

    //===========================[End of Fire]========================================

    //===========================[Ammo]===============================================

    public void OnNextAmmoPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !changingAmmo)
        {
            currentAmmoIndex++;
            if(currentAmmoIndex >= carriedAmmo.Count)
            {
                currentAmmoIndex = 0;
            }

            changingAmmo = true;
            GameObject.FindGameObjectWithTag("base_ammoIndicator").GetComponent<Animator>().SetTrigger("Next");
            changeAmmo();
        }

        
    }

    public void OnPreviousAmmoPressed(InputAction.CallbackContext context)
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

    private void changeAmmo()
    {
        if (currentAmmoBehaviourPrefab != null)
        {
            currentAmmoBehaviourPrefab.GetComponent<AmmoBehaviour>().OnUnequip(this);
            Destroy(currentAmmoBehaviourPrefab);
        }

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
            Debug.Log("this.carriedAmmo.Count + i = " + this.carriedAmmo.Count + " + " + i);
            return this.carriedAmmo[this.carriedAmmo.Count + i];
        }
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

    public void modifyAmmoAmount(Ammo ammoToRemove, int amount)
    {
        int indexOfAmmo = -1;
        for(int i = 0; i < carriedAmmo.Count; i++)
        {
            if(carriedAmmo[i].name == ammoToRemove.name)
            {
                indexOfAmmo = i;
                break;
            }
        }

        if(indexOfAmmo < 0)
        {
            Debug.LogError("Error: Could not find Ammo with name " + ammoToRemove.name + " in the list of carried ammo.");
            return;
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
            }
            GameObject.FindGameObjectWithTag("base_ammoIndicator").GetComponent<AmmoIndicatorLogic>().setAmmoIndicator();
        }
    }

    //===========================[End of Ammo]========================================
}
