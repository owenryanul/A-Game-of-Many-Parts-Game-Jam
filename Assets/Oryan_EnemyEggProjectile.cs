using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_EnemyEggProjectile : Oryan_EnemyProjectile
{
    public Vector3 targetPosition;
    public GameObject eggPrefab;
    public int pos;

    // Update is called once per frame
    protected override void Update()
    {
        if((targetPosition - this.gameObject.transform.position).magnitude <= 0.5f)
        {
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().addEnemyToBattle(eggPrefab, pos);
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
        }
    }
}
