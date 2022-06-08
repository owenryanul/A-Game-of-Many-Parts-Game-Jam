using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Sample_DialogBoxLogic : MonoBehaviour, Interactable
{
    private List<Dialog> queuedLines;
    private bool dialogBoxVisible;

    [System.Serializable]
    public class Dialog
    {
        [TextArea]
        public string text;
        public Sprite portrait;

        public Dialog(string textIn, Sprite portraitIn)
        {
            this.text = textIn;
            this.portrait = portraitIn;
        }
    }

    private void OnEnable()
    {
        this.queuedLines = new List<Dialog>();
    }

    // Start is called before the first frame update
    void Start()
    {
        showNextLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogBoxVisible)
        {
            GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>().setCurrentInteractable(this);
        }
    }

    public void showNextLine()  
    {
        if (dialogBoxVisible)
        {
            this.queuedLines.RemoveAt(0);
        }

        if (this.queuedLines.Count > 0)
        {
            dialogBoxVisible = true;
            this.gameObject.transform.Find("Background").GetComponent<Image>().enabled = true;            
            this.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

            Dialog currentDialog = this.queuedLines[0];
            
            this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentDialog.text;

            if (currentDialog.portrait != null)
            {
                this.gameObject.transform.Find("Portrait").GetComponent<Image>().enabled = true;
                this.gameObject.transform.Find("Portrait").GetComponent<Image>().sprite = currentDialog.portrait;
            }
            else
            {
                this.gameObject.transform.Find("Portrait").GetComponent<Image>().enabled = false;
            }            
        }
        else
        {
            dialogBoxVisible = false;
            this.gameObject.transform.Find("Background").GetComponent<Image>().enabled = false;
            this.gameObject.transform.Find("Portrait").GetComponent<Image>().enabled = false;
            this.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }
    }

    public void addDialogToQueue(string textIn, Sprite speakerIn)
    {
        this.queuedLines.Add(new Dialog(textIn, speakerIn));
    }

    public void addDialogToQueue(Dialog dialogIn)
    {
        this.queuedLines.Add(new Dialog(dialogIn.text, dialogIn.portrait));
    }

    public void onInteracted()
    {
        showNextLine();
    }

    public void onBecomeCurrentInteractable()
    {
        //No Effect
    }

    public void onNoLongerCurrentInteractable()
    {
        //No Effect
    }
}
