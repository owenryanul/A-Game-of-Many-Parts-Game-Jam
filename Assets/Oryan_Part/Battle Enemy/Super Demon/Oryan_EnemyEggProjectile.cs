using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_EnemyEggProjectile : Oryan_EnemyProjectile
{
    public Vector3 targetPosition;
    public GameObject eggPrefab;
    private int pos;

    protected override void Start()
    {
        base.Start();
        foreach(GameObject anEnemy in GameObject.FindGameObjectsWithTag("oryan_battleEnemy"))
        {
            if(anEnemy.GetComponent<Oryan_BasicBattleEnemy>().getBattlePosition() == pos)
            {
                parentEnemy.childProjectileDestroyed();
                Destroy(this.gameObject);
                break;
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if((targetPosition - this.gameObject.transform.position).magnitude <= 0.5f)
        {
            GameObject egg = GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().addEnemyToBattle(eggPrefab, pos);
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public void setPos(int posIn)
    {
        this.pos = posIn;
    }
}
