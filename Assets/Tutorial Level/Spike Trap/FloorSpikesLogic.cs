using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikesLogic : MonoBehaviour
{
    private bool playerColliding;
    private bool tripped;
    private bool stabAnimationCalled;
    private bool retractAnimationCalled;
    private bool spikesAreOut;
    public float tripDelay;
    public float retractDelay;
    private float timer;


    // Start is called before the first frame update
    void Start()
    {
        playerColliding = false;
        tripped = false;
        stabAnimationCalled = false;
        retractAnimationCalled = false;
        spikesAreOut = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(tripped && (!stabAnimationCalled || !retractAnimationCalled))
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                if(!stabAnimationCalled)
                {
                    stabAnimationCalled = true;
                    this.gameObject.GetComponent<Animator>().SetBool("Triggered", true);
                    timer = retractDelay;
                }
                else if(!retractAnimationCalled)
                {
                    retractAnimationCalled = true;
                    spikesAreOut = false;
                    this.gameObject.GetComponent<Animator>().SetBool("Triggered", false);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!tripped && collision.tag == "base_player")
        {
            playerColliding = true;
            tripped = true;
            timer = tripDelay;
        }

        if (tripped && spikesAreOut && collision.tag == "base_player" && !collision.gameObject.GetComponent<Player_Logic>().getIsDodging())
        {
            collision.gameObject.GetComponent<Player_Logic>().addHp(-1, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            playerColliding = false;
        }
    }

    public void OnStabAnimationDone()
    {
        Player_Logic player = GameObject.FindGameObjectWithTag("base_player").GetComponent<Player_Logic>();
        spikesAreOut = true;
        if(playerColliding && !player.getIsDodging())
        {
            player.addHp(-1, true);
        }
    }

    public void OnRetractAnimationDone()
    {
        //Fully reset trap
        stabAnimationCalled = false;
        retractAnimationCalled = false;
        tripped = false;
    }
}
