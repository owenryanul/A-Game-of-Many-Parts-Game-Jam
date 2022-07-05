using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_AmmoPickup : MonoBehaviour
{
    public Ammo ammo;
    public QuiverType type;

    public enum QuiverType
    {
        attack,
        defence,
        overworld
    }

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
        if (collision.tag == "base_player")
        {
            if (type == QuiverType.overworld)
            {
                collision.gameObject.GetComponent<PlayerLogic>().addAmmo(ammo);
            }
            else if(type == QuiverType.attack)
            {
                GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().attackQuiver.Add(ammo);
            }
            else if(type == QuiverType.defence)
            {
                GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().defenceQuiver.Add(ammo);
            }
            
            Destroy(this.gameObject);
        }
    }
}
