using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_DisableTrigger : MonoBehaviour
{
    private bool hasBeenTriggered;
    public bool triggerOnce;
    public List<GameObject> objects;

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
        if (!hasBeenTriggered && collision.tag == "base_player")
        {
            foreach(GameObject aGameObject in objects)
            {
                aGameObject.SetActive(false);
            }


            if (triggerOnce)
            {
                hasBeenTriggered = true;
            }
        }
    }
}
