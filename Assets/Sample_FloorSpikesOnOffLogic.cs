using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_FloorSpikesOnOffLogic : Sample_LeverListener
{

    public bool startWithSpikesOut;
    public float staggerForce;
    public Sprite spikesOnSprite;
    public Sprite spikesOffSprite;
    private bool spikesOut;
    private Vector2 staggerVector;
    public bool playerColliding;

    



    // Start is called before the first frame update
    void Start()
    {
        spikesOut = startWithSpikesOut;
        playerColliding = false;
        staggerVector = Vector2.zero;
        if (spikesOut)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = spikesOnSprite;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = spikesOffSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spikesOut && collision.tag == "base_player" && !collision.gameObject.GetComponent<PlayerLogic>().getIsDodging())
        {
            playerColliding = true;
            staggerVector = (collision.gameObject.transform.position - this.gameObject.transform.position).normalized * staggerForce;
            collision.gameObject.GetComponent<PlayerLogic>().addExternalVelocity(staggerVector);
            collision.gameObject.GetComponent<PlayerLogic>().addHp(0, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            if(playerColliding)
            {
                collision.gameObject.GetComponent<PlayerLogic>().addExternalVelocity(-staggerVector);
            }
            playerColliding = false;
        }
    }


    public override void onLeverStateChanged(bool isNowOn)
    {
        if (isNowOn)
        {
            spikesOut = false;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = spikesOffSprite;
        }
        else
        {
            spikesOut = true;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = spikesOnSprite;
        }

    }
}
