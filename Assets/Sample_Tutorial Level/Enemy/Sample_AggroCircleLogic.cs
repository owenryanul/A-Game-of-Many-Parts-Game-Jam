using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_AggroCircleLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            this.gameObject.transform.parent.GetComponent<Sample_basicEnemyLogic>().isAggroed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            this.gameObject.transform.parent.GetComponent<Sample_basicEnemyLogic>().isAggroed = false;
        }
    }
}
