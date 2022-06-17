using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_AggroCircle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            this.gameObject.transform.parent.GetComponent<Oryan_BasicOverworldEnemy>().isAggroed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            this.gameObject.transform.parent.GetComponent<Oryan_BasicOverworldEnemy>().isAggroed = false;
        }
    }
}
