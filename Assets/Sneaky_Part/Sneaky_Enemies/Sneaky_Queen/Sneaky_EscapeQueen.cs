using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sneaky_EscapeQueen : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            this.gameObject.transform.parent.GetComponent<Sneaky_Queen>().isAggroed = false;
        }
    }
}
