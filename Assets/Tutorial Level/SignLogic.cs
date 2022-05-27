using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignLogic : MonoBehaviour, Interactable
{
    public List<DialogBoxLogic.Dialog> signDialog;
    private DialogBoxLogic dialogBox;

    // Start is called before the first frame update
    void Start()
    {
        dialogBox = GameObject.FindGameObjectWithTag("tutorial_dialogBox").GetComponent<DialogBoxLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onBecomeCurrentInteractable()
    {
        this.GetComponentInChildren<SpriteMask>().enabled = true;
    }

    public void onInteracted()
    {
        foreach (DialogBoxLogic.Dialog aDialog in signDialog)
        {
            dialogBox.addDialogToQueue(aDialog);
        }
        dialogBox.showNextLine();
    }

    public void onNoLongerCurrentInteractable()
    {
        this.GetComponentInChildren<SpriteMask>().enabled = false;
    }
}
