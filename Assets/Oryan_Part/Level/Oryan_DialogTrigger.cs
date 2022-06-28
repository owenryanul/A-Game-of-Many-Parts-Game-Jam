using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_DialogTrigger : MonoBehaviour
{
    [TextArea]
    public List<string> dialogText;
    public List<Sprite> dialogSprite;

    private bool hasBeenTriggered;

    // Start is called before the first frame update
    void Start()
    {
        hasBeenTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Oryan_DialogBox dialogBox = GameObject.FindGameObjectWithTag("oryan_dialogBox").GetComponent<Oryan_DialogBox>();
        if (!hasBeenTriggered && collision.tag == "base_player")
        {
            for(int i = 0; i < dialogText.Count; i++)
            {
                dialogBox.addDialogToQueue(dialogText[i], dialogSprite[i]);
            }
            dialogBox.showNextLine();
            hasBeenTriggered = true;
        }
    }
}
