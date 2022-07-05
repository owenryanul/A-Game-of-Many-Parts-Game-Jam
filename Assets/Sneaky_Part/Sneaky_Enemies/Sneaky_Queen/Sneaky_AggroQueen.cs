using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sneaky_AggroQueen : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            this.gameObject.transform.parent.GetComponent<Sneaky_Queen>().isAggroed = true;
        }
    }
}
