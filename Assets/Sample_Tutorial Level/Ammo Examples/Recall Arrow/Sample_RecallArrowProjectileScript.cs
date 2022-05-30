using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_RecallArrowProjectileScript : MonoBehaviour, OnActivateListener
{
    public float maxSpeed;
    public float facingOffset; //inDegrees
    public Ammo ammo;
    private float currentReturnSpeed;

    private Vector3 targetDirection;

    public float returnAcceleration;
    private bool isRecalling;

    // Start is called before the first frame update
    void Start()
    {
        isRecalling = false;
        GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>().addOnActivateListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(isRecalling)
        {
            currentReturnSpeed += returnAcceleration * Time.deltaTime;
            if(currentReturnSpeed > maxSpeed)
            {
                currentReturnSpeed = maxSpeed;
            }
            Vector3 vectorToPlayer = (GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position).normalized;
            this.gameObject.GetComponent<Rigidbody2D>().velocity = vectorToPlayer * currentReturnSpeed;
        }
        else
        {
            currentReturnSpeed -= returnAcceleration * Time.deltaTime;
            if (currentReturnSpeed < 0)
            {
                currentReturnSpeed = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "base_player" && isRecalling)
        {
            collision.gameObject.GetComponent<PlayerLogic>().addAmmo(ammo);
            Destroy(this.gameObject);
        }
    }

    public void fireInDirection(Vector3 targetDirectionIn)
    {
        targetDirection = targetDirectionIn;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        this.gameObject.GetComponent<Rigidbody2D>().rotation = angle + facingOffset;

        this.gameObject.GetComponent<Rigidbody2D>().velocity = targetDirection * maxSpeed;
    }

    public void OnActivatePressed()
    {
        this.isRecalling = true;
    }

    public void OnActivateReleased()
    {
        this.isRecalling = false;
    }

    private void OnDestroy()
    {
        if (GameObject.FindGameObjectWithTag("base_player") != null)
        {
            GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>().removeOnActivateListener(this);
        }
    }
}
