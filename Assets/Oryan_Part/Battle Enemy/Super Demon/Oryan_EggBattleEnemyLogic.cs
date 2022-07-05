using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_EggBattleEnemyLogic : Oryan_BasicBattleEnemy
{
    public int hatchTime;
    private int turnsLeftUntilHatch;
    public GameObject particleEmitterPrefab;
    public GameObject cultistPrefab;
    public GameObject demonPrefab;

    protected override void Start()
    {
        base.Start();
        turnsLeftUntilHatch = hatchTime;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
    }

    public override void makeAttack()
    {
        turnsLeftUntilHatch--;
        if(turnsLeftUntilHatch <= 0)
        {
            switch(Random.Range(1, 3))
            {
                case 1: GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().addEnemyToBattle(cultistPrefab, base.getBattlePosition()); break;
                case 2: GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().addEnemyToBattle(demonPrefab, base.getBattlePosition()); break;
            }
            
            Instantiate(particleEmitterPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
            base.kill(true);
        }
    }
}
