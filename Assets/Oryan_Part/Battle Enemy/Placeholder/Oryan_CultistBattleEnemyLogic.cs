using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_CultistBattleEnemyLogic : Oryan_BasicBattleEnemy
{
    private int knives;
    private bool isSpraying;

    public GameObject sprayProjectile;
    public Vector3 sprayOffset;

    public bool knivesOnly;

    public float fallshotLifeTime1;
    public float fallshotLifeTime2;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        isSpraying = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(isSpraying)
        {
            Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - (this.gameObject.transform.position + sprayOffset);
            GameObject bolt = Instantiate(sprayProjectile, this.gameObject.transform.position + sprayOffset, sprayProjectile.transform.rotation);
            bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
            bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayer);
            activeChildProjectiles++;
        }
    }

    public override void makeAttack()
    {
        base.isPlayingAttackAnimation = true;
        base.hasHurtPlayerThisAttack = false;

        int rand = Random.Range(0, 2);

        if(rand == 0 || knivesOnly)
        {
            int rand2 = Random.Range(1, 4);
            knives = rand2;
            this.gameObject.GetComponent<Animator>().SetInteger("Knives", knives);
            this.gameObject.GetComponent<Animator>().SetTrigger("Attack");
        }
        else
        {
            knives = 4;
            this.gameObject.GetComponent<Animator>().SetInteger("Knives", knives);
            this.gameObject.GetComponent<Animator>().SetTrigger("Attack");
        }
    }

    public void throwKnife()
    {
        Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
        GameObject bolt = Instantiate(projectile, this.gameObject.transform.position + sprayOffset, projectile.transform.rotation);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().setProjectileParent(this);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().fireInDirection(directionTowardsPlayer);
        activeChildProjectiles++;
    }

    public void throwBadKnife()
    {
        Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
        GameObject bolt = Instantiate(projectile, this.gameObject.transform.position + sprayOffset, projectile.transform.rotation);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().setProjectileParent(this);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().fireInDirection(directionTowardsPlayer);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().fallShort = true;
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().fallShortDuration = fallshotLifeTime1;//bolt.GetComponent<Oryan_EnemyKnifeProjectile>().totalLifetime / 2;
        activeChildProjectiles++;
    }

    public void throwOtherBadKnife()
    {
        Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
        GameObject bolt = Instantiate(projectile, this.gameObject.transform.position + sprayOffset, projectile.transform.rotation);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().setProjectileParent(this);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().fireInDirection(directionTowardsPlayer);
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().fallShort = true;
        bolt.GetComponent<Oryan_EnemyKnifeProjectile>().fallShortDuration = fallshotLifeTime2; //bolt.GetComponent<Oryan_EnemyKnifeProjectile>().totalLifetime / 3;
        activeChildProjectiles++;
    }
    
    public void startSpray()
    {
        isSpraying = true;
    }

    public void stopSpray()
    {
        isSpraying = false;
    }

    public override void onAttachedParryTargetHit(GameObject source)
    {
        this.gameObject.GetComponent<Animator>().SetTrigger("Hurt");
        this.isPlayingAttackAnimation = false;
    }
}
