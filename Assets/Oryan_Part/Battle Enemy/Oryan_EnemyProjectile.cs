using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_EnemyProjectile : MonoBehaviour
{
    public float speed;
    public float facingOffset; //inDegrees
    public float totalLifetime;
    public int damage;
    public bool dodgeable;
    public bool parryable;

    private float currentLifetime;
    private Oryan_ProjectileParent parentEnemy;

    private Vector3 targetDirection;

    // Start is called before the first frame update
    void Start()
    {
        currentLifetime = totalLifetime;
    }

    // Update is called once per frame
    void Update()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "base_player" && (!this.dodgeable || (this.dodgeable && !collision.gameObject.GetComponent<PlayerLogic>().getIsDodging())))
        {
            if (!dodgeable && collision.gameObject.GetComponent<PlayerLogic>().getIsDodging())
            {
                collision.gameObject.GetComponent<PlayerLogic>().addHp(-damage, false);
                //===================play replacement hurt sound here.
            }
            else
            {
                collision.gameObject.GetComponent<PlayerLogic>().addHp(-damage, true);
            }
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
        }
        else if(collision.tag == "oryan_parryArrow" && this.parryable)
        {
            parentEnemy.childProjectileDestroyed();
            Destroy(this.gameObject);
            Destroy(collision.gameObject);
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
