using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_AggroCircle : MonoBehaviour
{
    private GameObject player;
    private Oryan_GlobalDataHolder litStatus;
    private bool checkingForLineOfSight;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player");
        litStatus = GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_GlobalDataHolder>();
        checkingForLineOfSight = false;
    }

    void Update()
    {
        if (checkingForLineOfSight && !this.gameObject.transform.parent.GetComponent<Oryan_BasicOverworldEnemy>().getIsAggroed())
        {
            Debug.Log("Checking los");
            float castDistance = this.gameObject.GetComponent<CircleCollider2D>().radius * this.gameObject.transform.parent.localScale.x;
            if (!litStatus.isThePlayerLit())
            {
                castDistance = (castDistance / 2);
            }

            RaycastHit2D hit = Physics2D.Raycast(this.gameObject.transform.position, player.transform.position - this.gameObject.transform.position, castDistance, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Level"));
            if(hit && hit.collider.tag == "base_player")
            {
                Debug.Log("Aggroing");
                this.gameObject.transform.parent.GetComponent<Oryan_BasicOverworldEnemy>().setAggroed(true);
            }
            else if(hit)
            {
                Debug.Log("Hit something" + hit.collider.name);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            checkingForLineOfSight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            checkingForLineOfSight = false;
            this.gameObject.transform.parent.GetComponent<Oryan_BasicOverworldEnemy>().setAggroed(false);
        }
    }
}
