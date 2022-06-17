using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_BasicBattleEnemy : Oryan_TurnTaker
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (base.isTheirTurn)
        {
            base.endTheirTurn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "oryan_playerAttackProjectile")
        {
            base.removeThemFromTurnTakers();
            Destroy(this.gameObject);
        }
    }

    public override void onTheirTurnStart()
    {
        Debug.Log("OnTheirTurnStarted " + this.gameObject.name);
        base.onTheirTurnStart();
    }
}
