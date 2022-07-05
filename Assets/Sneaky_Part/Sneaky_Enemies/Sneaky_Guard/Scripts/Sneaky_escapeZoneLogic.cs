using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sneaky_escapeZoneLogic : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            this.gameObject.transform.parent.GetComponent<Sneaky_GuardLogic>().isAggroed = false;
        }
    }
}
