using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_DoorLogic : Sample_LeverListener
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void onLeverStateChanged(bool isNowOn)
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = !isNowOn;
        this.gameObject.GetComponent<Collider2D>().enabled = !isNowOn;
    }
}
