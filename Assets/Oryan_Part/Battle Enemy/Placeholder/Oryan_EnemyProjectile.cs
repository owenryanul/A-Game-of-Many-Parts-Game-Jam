using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_EnemyProjectile : MonoBehaviour
{
    public float speed;
    public float facingOffset; //inDegrees
    public float totalLifetime;
    public int damage;
    public bool damageOncePerTurn;
    public bool dodgeable;
    public bool parryable;
    public bool blockable;

    protected float currentLifetime;
    protected Oryan_ProjectileParent parentEnemy;

    protected Vector3 targetDirection;

    public AudioClip playerPainSound;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentLifetime = totalLifetime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (currentLifetime <= 0)
        {
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
        }
        else
        {
            currentLifetime -= Time.deltaTime;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "oryan_playerShield")
        {
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
        }

       
        if(collision.tag == "base_player")
        {
            //Not dodged or undodgeable
            if(!this.dodgeable || (this.dodgeable && !collision.gameObject.GetComponent<PlayerLogic>().getIsDodging()))
            {
                //Has not given i-frames
                if (!damageOncePerTurn || (damageOncePerTurn && !parentEnemy.hasChildProjectilesDamagedPlayerThisAttack()))
                {
                    if (!dodgeable && collision.gameObject.GetComponent<PlayerLogic>().getIsDodging())
                    {
                        //damage dodging player
                        collision.gameObject.GetComponent<PlayerLogic>().addHp(-damage, false);
                        this.gameObject.GetComponent<AudioSource>().PlayOneShot(playerPainSound, 1);
                        //===================play replacement hurt sound here.
                    }
                    else
                    {
                        //damage player
                        collision.gameObject.GetComponent<PlayerLogic>().addHp(-damage, true);
                    }
                }


                parentEnemy.childProjectileDamagedPlayer();
                parentEnemy.childProjectileDestroyed();
                Destroy(this.gameObject);
            }
        }
        else if(collision.tag == "oryan_parryArrow" && this.parryable)
        {
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
            Destroy(collision.gameObject);
        }

        if(collision.tag == "oryan_battleEdge")
        {
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
        }
    }

    public void fireInDirection(Vector3 targetDirectionIn)
    {
        targetDirection = targetDirectionIn.normalized;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        this.gameObject.GetComponent<Rigidbody2D>().rotation = angle + facingOffset;

        this.gameObject.GetComponent<Rigidbody2D>().velocity = targetDirection * speed;
    }

    public void setProjectileParent(Oryan_ProjectileParent parentIn)
    {
        parentEnemy = parentIn;
    }
}
