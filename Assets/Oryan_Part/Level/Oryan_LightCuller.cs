using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_LightCuller : MonoBehaviour
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
        if (collision.tag == "oryan_lightCuller")
        {
            this.gameObject.transform.parent.GetComponentInChildren<Light>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "oryan_lightCuller")
        {
            this.gameObject.transform.parent.GetComponentInChildren<Light>().enabled = false;
        }
    }
}
