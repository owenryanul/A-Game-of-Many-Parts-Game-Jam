using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_AggroDoor : MonoBehaviour
{
    public List<Oryan_BasicOverworldEnemy> watchers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool allClear = true;
        foreach (Oryan_BasicOverworldEnemy anEnemy in watchers)
        {
            if(anEnemy.getIsAggroed())
            {
                this.gameObject.transform.Find("Gate").gameObject.GetComponent<SpriteRenderer>().enabled = true;
                this.gameObject.transform.Find("Gate").gameObject.GetComponent<Collider2D>().enabled = true;
                allClear = false;
                break;
            }
        }
        
        if(allClear)
        {
            this.gameObject.transform.Find("Gate").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.transform.Find("Gate").gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
