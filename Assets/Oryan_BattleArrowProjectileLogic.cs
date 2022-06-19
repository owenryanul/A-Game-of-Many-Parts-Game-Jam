using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_BattleArrowProjectileLogic : MonoBehaviour
{
    public float speed;
    public float facingOffset; //inDegrees
    public float minVelocityBeforePickupable;
    public float lifetime;
    public int damage;

    private Vector3 targetDirection;
    private Oryan_ProjectileParent projectileParent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude <= minVelocityBeforePickupable)
        {
            die();
        }

        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
        {
            die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "oryan_battleEnemy")
        {
            collision.gameObject.GetComponent<Oryan_BasicBattleEnemy>().dealDamage(this.damage);
            die();
        }
        
    }

    private void die()
    {   
        projectileParent.childProjectileDestroyed();
        Destroy(this.gameObject);
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
        this.projectileParent = parentIn;
    }
}
