using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_SuperDemonBattleEnemyLogic : Oryan_BasicBattleEnemy
{
    [Header("Charge Attack")]
    public Vector3 chargeStartOffsetFromPlayer;
    public float walkSpeed;
    public float chargeSpeed;
    public int chargeDamage;
    public int maxChargeAttempts;
    private int currentChargeAttempts;
    private Vector3 startPos;
    private bool isMakingChargeAttack;
    private bool isWindingUpCharging;
    private bool isCharging;
    private bool isStaggered;
    private bool isResettingAfterCharge;
    public bool onlyUseChargeAttack = false;


    [Header("Spray")]
    public GameObject sprayProjectile;
    public Vector3 sprayOffset;
    public float sprayRotateDegreesPerSecond;
    public float sprayMaxAngle;
    private float currentSprayAngle;
    private bool sprayAngleReveresed;
    private bool isSpraying;
    private float timeSinceLastSprayProjectile;

    [Header("Eggs")]
    public GameObject eggProjectile;

    public GameObject shockwaveProjectile;

    private GameObject player;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        isSpraying = false;
        timeSinceLastSprayProjectile = 0;
        isMakingChargeAttack = false;
        isCharging = false;
        isStaggered = false;
        isResettingAfterCharge = false;
        isWindingUpCharging = false;
        currentChargeAttempts = 0;
        startPos = this.gameObject.transform.position;
        this.player = GameObject.FindGameObjectWithTag("base_player");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        if(isMakingChargeAttack)
        {
            chargeAttackUpdate();
        }

        if (isSpraying)
        {
            if (sprayAngleReveresed)
            {
                currentSprayAngle -= sprayRotateDegreesPerSecond * Time.deltaTime;
                if (currentSprayAngle <= -sprayMaxAngle)
                {
                    sprayAngleReveresed = false;
                    currentSprayAngle = -sprayMaxAngle;
                }
            }
            else
            {
                currentSprayAngle += sprayRotateDegreesPerSecond * Time.deltaTime;
                if (currentSprayAngle >= sprayMaxAngle)
                {
                    sprayAngleReveresed = true;
                    currentSprayAngle = sprayMaxAngle;
                }
            }

            Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - (this.gameObject.transform.position + sprayOffset);
            Vector3 directionTowardsPlayerAfterRotation = Quaternion.AngleAxis(currentSprayAngle, this.transform.forward) * directionTowardsPlayer;

            GameObject bolt = Instantiate(sprayProjectile, this.gameObject.transform.position + sprayOffset, sprayProjectile.transform.rotation);
            bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
            bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayerAfterRotation);
            activeChildProjectiles++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMakingChargeAttack && collision.tag == "base_player")
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("ChargeReset");
            isResettingAfterCharge = true;
            if (collision.gameObject.GetComponent<PlayerLogic>().getIsDodging())
            {
                //damage dodging player
                collision.gameObject.GetComponent<PlayerLogic>().addHp(-chargeDamage, false);
                //===================play replacement hurt sound here.
            }
            else
            {
                //damage player
                collision.gameObject.GetComponent<PlayerLogic>().addHp(-chargeDamage, true);
            }
        }
    }

    public override void makeAttack()
    {
        base.isPlayingAttackAnimation = true;
        base.hasHurtPlayerThisAttack = false;

        int rand = Random.Range(1, 5);
        if(onlyUseChargeAttack)
        {
            rand = 1;
        }
        this.gameObject.GetComponent<Animator>().SetInteger("AttackType", rand);
        this.gameObject.GetComponent<Animator>().SetTrigger("Attack");

        //charge
        switch(rand)
        {
            case 1: isMakingChargeAttack = true; break;
            case 2: /*animation will handle everything;*/ break;
            case 3: /*animation will handle;*/ break;
            case 4: /*animation will handle;*/ break;
        }

    }


    private void chargeAttackUpdate()
    {
        if(isResettingAfterCharge)
        {
            //Walk back to start point
            Vector3 target = startPos;
            target.z = 0;
            this.gameObject.transform.Find("Master Sprite").Find("Demon Sprite").GetComponent<SpriteRenderer>().flipX = false;

            Vector2 direction = target - transform.position;
            if (direction.magnitude <= (walkSpeed * Time.deltaTime) + 0.01f)
            {
                this.gameObject.transform.Find("Master Sprite").Find("Demon Sprite").GetComponent<SpriteRenderer>().flipX = true;
                this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                this.gameObject.GetComponent<Rigidbody2D>().MovePosition(target);
                isMakingChargeAttack = false;
                isCharging = false;
                isStaggered = false;
                isResettingAfterCharge = false;
                isWindingUpCharging = false;
                currentChargeAttempts = 0;
                this.gameObject.GetComponent<Animator>().SetTrigger("FinishCharge");
                base.isPlayingAttackAnimation = false;
            }
            else
            {
                direction.Normalize();
                this.gameObject.GetComponent<Rigidbody2D>().velocity = (direction * walkSpeed);
            }
        }
        else if (isCharging)
        {
            //Charge towards the player
            Vector3 target = player.transform.position;
            target.z = 0;

            Vector2 direction = target - transform.position;
            direction.Normalize();
            if (isStaggered)
            {
                //Move away from player in respond to being hit.
                this.gameObject.GetComponent<Rigidbody2D>().velocity = (direction * -chargeSpeed);
            }
            else
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = (direction * chargeSpeed);
            }
        }
        else if (isWindingUpCharging)
        {
            //do nothing while the animation plays out
        }
        else
        {
            //Moving into windup position
            Vector3 target = player.transform.position + chargeStartOffsetFromPlayer;
            target.z = 0;

            Vector2 direction = target - transform.position;
            if (direction.magnitude <= (walkSpeed * Time.deltaTime) + 0.01f)
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                this.gameObject.GetComponent<Rigidbody2D>().MovePosition(target);
                this.isWindingUpCharging = true;
                this.gameObject.GetComponent<Animator>().SetTrigger("ChargeWindup");
            }
            else
            {
                direction.Normalize();
                this.gameObject.GetComponent<Rigidbody2D>().velocity = (direction * walkSpeed);
            }
        }
    }

    public void OnChargeWindupDone()
    {
        isCharging = true;
    }

    public void OnChargeRecovered()
    {
        isStaggered = false;
        currentChargeAttempts++;
        if(currentChargeAttempts >= maxChargeAttempts)
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("ChargeReset");
            isResettingAfterCharge = true;
        }
    }

    public override void onAttachedParryTargetHit(GameObject source)
    {
        if(isCharging)
        {
            isStaggered = true;
            this.gameObject.GetComponent<Animator>().SetTrigger("ChargeStagger");
        }
    }





    public void launchShockwave()
    {
        Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
        GameObject bolt = Instantiate(shockwaveProjectile, this.gameObject.transform.position, shockwaveProjectile.transform.rotation);
        bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
        bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayer);
        activeChildProjectiles++;
    }





    public void startSpray()
    {
        isSpraying = true;
        currentSprayAngle = sprayMaxAngle;
        sprayAngleReveresed = false;
    }

    public void stopSpray()
    {
        isSpraying = false;
    }




    public void fireEgg1()
    {
        Vector3 pos = GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().enemySpots[0].transform.position;
        GameObject bolt = Instantiate(eggProjectile, this.gameObject.transform.position, this.gameObject.transform.rotation);
        bolt.GetComponent<Oryan_EnemyEggProjectile>().fireInDirection(pos - this.gameObject.transform.position);
        bolt.GetComponent<Oryan_EnemyEggProjectile>().setProjectileParent(this);
        base.activeChildProjectiles++;
        bolt.GetComponent<Oryan_EnemyEggProjectile>().targetPosition = pos;
        bolt.GetComponent<Oryan_EnemyEggProjectile>().setPos(0);
    }

    public void fireEgg2()
    {
        Vector3 pos = GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().enemySpots[2].transform.position;
        GameObject bolt = Instantiate(eggProjectile, this.gameObject.transform.position, this.gameObject.transform.rotation);
        bolt.GetComponent<Oryan_EnemyEggProjectile>().fireInDirection(pos - this.gameObject.transform.position);
        bolt.GetComponent<Oryan_EnemyEggProjectile>().setProjectileParent(this);
        base.activeChildProjectiles++;
        bolt.GetComponent<Oryan_EnemyEggProjectile>().targetPosition = pos;
        bolt.GetComponent<Oryan_EnemyEggProjectile>().setPos(2);
    }

    //Not to be confused with kill(). onKill() is called before the rest of kill()'s logic runs, allowing for any on death effects to trigger.
    public override void onKill()
    {
        foreach (GameObject anEnemy in GameObject.FindGameObjectsWithTag("oryan_battleEnemy"))
        {
            if (anEnemy.GetComponent<Oryan_BasicBattleEnemy>() != this)
            {
                anEnemy.GetComponent<Oryan_BasicBattleEnemy>().dealDamage(99999);
            }
        }
    }


    private void OnDestroy()
    {

    }
}
