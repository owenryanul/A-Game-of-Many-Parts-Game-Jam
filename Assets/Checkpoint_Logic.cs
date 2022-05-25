using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint_Logic : MonoBehaviour, OnDeathListener
{
    public static Checkpoint_Logic activeCheckpoint;


    // Start is called before the first frame update
    void Start()
    {
        if(activeCheckpoint == null)
        {
            activeCheckpoint = this;
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            GameObject.FindGameObjectWithTag("base_player").GetComponent<Player_Logic>().addOnDeathListener(this);
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "base_player")
        {
            this.makeActivateCheckpoint();
        }
    }

    private void makeActivateCheckpoint()
    {
        if (activeCheckpoint != null)
        {
            GameObject.FindGameObjectWithTag("base_player").GetComponent<Player_Logic>().removeOnDeathListener(activeCheckpoint);
            activeCheckpoint.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        activeCheckpoint = this;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        GameObject.FindGameObjectWithTag("base_player").GetComponent<Player_Logic>().addOnDeathListener(this);
    }

    public void OnPlayerDies(Player_Logic playerLogic)
    {
        playerLogic.gameObject.GetComponent<Rigidbody2D>().MovePosition(this.gameObject.transform.position);
        playerLogic.respawn();
        playerLogic.setHp(3, false);
    }
}
