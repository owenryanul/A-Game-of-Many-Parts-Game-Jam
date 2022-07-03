using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_CheckPointTrigger : MonoBehaviour
{
    private bool hasBeenTriggered;
    public bool triggerOnce;
    public GameObject checkpoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenTriggered && collision.tag == "base_player")
        {
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().activeCheckpointPosition = checkpoint.transform.position;
            if (triggerOnce)
            {
                hasBeenTriggered = true;
            }
        }
    }
}
