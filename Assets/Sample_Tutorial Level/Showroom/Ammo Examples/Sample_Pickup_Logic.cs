using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_Pickup_Logic : MonoBehaviour
{
    public Ammo ammo;

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
        if(collision.tag == "base_player")
        {
            collision.gameObject.GetComponent<PlayerLogic>().addAmmo(ammo);
            Destroy(this.gameObject);
        }
    }
}
