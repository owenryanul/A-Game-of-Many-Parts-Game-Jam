using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_LightDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "base_player")
        {
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_GlobalDataHolder>().lightSourcesLightingThePlayer++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_GlobalDataHolder>().lightSourcesLightingThePlayer--;
        }
    }
}
