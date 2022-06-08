using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_ExplosionLogic : MonoBehaviour
{
    public float explosionDuration;
    private float explosionLifetimeRemaining;

    // Start is called before the first frame update
    void Start()
    {
        explosionLifetimeRemaining = explosionDuration;
    }

    // Update is called once per frame
    void Update()
    {
        explosionLifetimeRemaining -= Time.deltaTime;
        if(explosionLifetimeRemaining <= 0)
        {
            this.gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "base_player")
        {
            collision.gameObject.GetComponent<PlayerLogic>().addHp(-1, true);
        }
        else if(collision.tag == "sample_blastable")
        {
            Destroy(collision.gameObject);
        }
    }
}
