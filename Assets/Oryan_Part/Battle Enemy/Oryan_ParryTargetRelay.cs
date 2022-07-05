using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_ParryTargetRelay : MonoBehaviour
{
    public GameObject parent;

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
        Debug.Log("Trigger enter " + collision.name);
        if(collision.tag == "oryan_parryArrow")
        {
            parent.GetComponent<Oryan_BasicBattleEnemy>().onAttachedParryTargetHit(this.gameObject);
        }
    }
}
