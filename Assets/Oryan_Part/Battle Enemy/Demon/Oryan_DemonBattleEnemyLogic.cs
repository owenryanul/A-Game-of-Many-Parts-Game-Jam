using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_DemonBattleEnemyLogic : Oryan_BasicBattleEnemy
{
    private bool isSpraying;

    public GameObject sprayProjectile;
    public Vector3 sprayOffset;

    public GameObject shieldPrefab;
    private GameObject shield;

    public GameObject shockwaveProjectile;

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
        if (isSpraying)
        {
            Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
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

        int rand = Random.Range(1, 4);
        this.gameObject.GetComponent<Animator>().SetInteger("AttackType", rand);
        this.gameObject.GetComponent<Animator>().SetTrigger("Attack");

    }

    public void launchShockwave()
    {
        Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
        GameObject bolt = Instantiate(shockwaveProjectile, this.gameObject.transform.position, shockwaveProjectile.transform.rotation);
        bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
        bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayer);
        activeChildProjectiles++;
    }

    public void spawnShield()
    {
        if (shield == null)
        {
            shield = Instantiate(shieldPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
        }
        else
        {
            shield.GetComponent<Oryan_ParryShieldLogic>().restoreShields();
        }
    }

    public void startSpray()
    {
        isSpraying = true;
    }

    public void stopSpray()
    {
        isSpraying = false;
    }

    private void OnDestroy()
    {
        if (shield != null)
        {
            Destroy(shield);
        }
    }
}
