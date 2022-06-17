using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_BasicOverworldEnemy : MonoBehaviour
{
    public bool isAggroed;
    public float speed = 1;
    public GameObject deathParticleEmitter;
    public bool runTowardsPlayer = false;

    private GameObject player;

    private bool dying;

    [Header("Encounter")]
    public List<GameObject> enemiesInEncounter;

    // Start is called before the first frame updateGetComponent<SpriteRenderer>()
    void Start()
    {
        this.player = GameObject.FindWithTag("base_player");
        dying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dying)
        {
            if (isAggroed && player != null)
            {
                Vector3 target = player.transform.position;
                target.z = 0;

                Vector2 direction = target - transform.position;
                direction.Normalize();

                //move in the direction you're facing
                if (runTowardsPlayer)
                {
                    this.gameObject.GetComponent<Rigidbody2D>().velocity = (direction * speed);

                    if (direction.x > 0)
                    {
                        this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = false;
                    }
                    else
                    {
                        this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = true;
                    }
                }
                else
                {
                    this.gameObject.GetComponent<Rigidbody2D>().velocity = (-direction * speed);

                    if (direction.x > 0)
                    {
                        this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = true;
                    }
                    else
                    {
                        this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = false;
                    }
                }
            }
            else if (!isAggroed)
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "base_player" && !player.GetComponent<PlayerLogic>().getIsDodging())
        {
            //Die to player
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().startBattle(this.enemiesInEncounter);
            die();
        }
        else if (collision.gameObject.tag == "sample_playerProjectile" || collision.gameObject.tag == "sample_explosion")
        {
            die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "sample_playerProjectile" || collision.gameObject.tag == "sample_explosion")
        {
            die();
        }
    }

    private void die()
    {
        if (!dying)
        {
            dying = true;
            Instantiate(deathParticleEmitter, this.gameObject.transform.position, this.gameObject.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
