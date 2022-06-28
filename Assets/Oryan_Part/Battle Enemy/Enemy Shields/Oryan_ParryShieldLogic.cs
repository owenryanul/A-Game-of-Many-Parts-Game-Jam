using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_ParryShieldLogic : MonoBehaviour
{
    public int maxNumberOfShields;
    private int currentNumberOfShields;

    // Start is called before the first frame update
    void Start()
    {
        currentNumberOfShields = maxNumberOfShields;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onShieldDown()
    {
        currentNumberOfShields--;
        if(currentNumberOfShields <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void restoreShields()
    {
        currentNumberOfShields = maxNumberOfShields;
        foreach(Collider2D collider2D in this.gameObject.GetComponentsInChildren<Collider2D>())
        {
            collider2D.enabled = true;
        }

        foreach (SpriteRenderer sprite in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.enabled = true;
        }
    }
}
