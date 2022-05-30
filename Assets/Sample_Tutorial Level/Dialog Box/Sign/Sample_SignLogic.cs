using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_SignLogic : MonoBehaviour, Interactable
{
    public List<Sample_DialogBoxLogic.Dialog> signDialog;
    private Sample_DialogBoxLogic dialogBox;

    // Start is called before the first frame update
    void Start()
    {
        dialogBox = GameObject.FindGameObjectWithTag("sample_dialogBox").GetComponent<Sample_DialogBoxLogic>();
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
        foreach (Sample_DialogBoxLogic.Dialog aDialog in signDialog)
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
