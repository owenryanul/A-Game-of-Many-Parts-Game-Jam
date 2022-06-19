using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_BasicBattleEnemy : Oryan_TurnTaker, Oryan_ProjectileParent
{
    public GameObject projectile;
    public GameObject parryProjectile;
    public bool doShoot = true;

    public int maxHp;
    private int currentHp;

    private bool destroyNextUpdate;

    // Start is called before the first frame update
    void Start()
    {
        maxHp = currentHp;
        destroyNextUpdate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(destroyNextUpdate)
        {
            kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public override void onTheirTurnStart()
    {
        Debug.Log("OnTheirTurnStarted " + this.gameObject.name);
        base.onTheirTurnStart();

        if (doShoot)
        {
            if(Random.Range(0, 2) == 0)
            {
                Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
                GameObject bolt = Instantiate(projectile, this.gameObject.transform.position, projectile.transform.rotation);
                bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
                bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayer);
            }
            else
            {
                Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
                GameObject bolt = Instantiate(parryProjectile, this.gameObject.transform.position, projectile.transform.rotation);
                bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
                bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayer);
            }
        }
    }

    public void dealDamage(int damageIn)
    {
        currentHp -= damageIn;
        if(currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        else if(currentHp <= 0)
        {
            kill();
        }
    }

    private void kill()
    {
            base.removeThemFromTurnTakers();
            Destroy(this.gameObject);
    }

    public void childProjectileDestroyed()
    {
        if(base.isTheirTurn)
        {
            base.endTheirTurn();
        }
    }

    public override void markForDeletion()
    {
        destroyNextUpdate = true;
    }
}
