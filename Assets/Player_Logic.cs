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
    private Vector3 movementDirection;
    private Vector3 lastMovementDirection;

    [Header("Aiming")]
    public Vector3 aimDirection;
    public float maxReticleDistanceFromPlayer;
    private bool usingMouse;

    public float force;

    // Start is called before the first frame update
    void Start()
    {
        movementDirection = Vector3.zero;
        currentSpeed = 0.0f;
        aimDirection = Vector3.right;
        usingMouse = false;
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
        if (movementDirection != Vector3.zero)
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
    }

    //Determines player speed based on how long they've been moving. They accelerate up to max speed based on acceleration while moving
    // and deccelerate over time while not moving. This is intended to give the player a sense of momentum.
    private void updateCurrentSpeed()
    {
        if (movementDirection == Vector3.zero)
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

    public void OnMovementKeysHit(InputAction.CallbackContext context)
    {
        this.movementDirection = context.ReadValue<Vector2>().normalized;
        if(this.movementDirection != Vector3.zero)
        {
            this.lastMovementDirection = this.movementDirection;
        }
        //Debug.Log("movement direction changed, is now: " + this.movementDirection);
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
        if(context.phase == InputActionPhase.Performed)
        {
            PressInteraction pressInteraction = (PressInteraction)context.interaction;
            if(pressInteraction.behavior == PressBehavior.PressOnly)
            {
                Debug.Log("Fist");
                this.gameObject.GetComponent<Rigidbody2D>().velocity += (Vector2)(aimDirection.normalized * force);
            }
            else if(pressInteraction.behavior == PressBehavior.ReleaseOnly)
            {
                
            }
        }
        else
        {

        }
    }

    //===========================[End of Fire]========================================
}
