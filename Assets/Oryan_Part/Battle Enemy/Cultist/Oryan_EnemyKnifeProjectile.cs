using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_EnemyKnifeProjectile : Oryan_EnemyProjectile
{
    public bool fallShort;
    public float fallShortDuration;
    public Sprite stuckSprite;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(this.currentLifetime <= fallShortDuration)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = stuckSprite;
        }
    }


}
