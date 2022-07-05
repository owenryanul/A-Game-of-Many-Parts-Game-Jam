using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sneaky_StingerScript : MonoBehaviour
{
    private GameObject target;

    public float speed;

    private Rigidbody2D bulletRB;
    // Start is called before the first frame update
    void Start()
    {
        bulletRB = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("base_player");
        Vector2 moveDir = (target.transform.position - transform.position).normalized * speed;
        bulletRB.velocity = new Vector2(moveDir.x, moveDir.y);
        Destroy(this.gameObject, 2);
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "base_player" && !target.GetComponent<PlayerLogic>().getIsDodging())
        {
            
            target.GetComponent<PlayerLogic>().addHp(-1, true);
            Destroy(this.gameObject);
        }
        
    }
   
}
