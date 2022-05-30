using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_LeverLogic : MonoBehaviour, Interactable
{
    [Header("Listener")]
    public List<Sample_LeverListener> listeners;

    [Header("Visuals")]
    public Sprite onSprite;
    public Sprite offSprite;
    private bool isOn;

    

    // Start is called before the first frame update
    void Start()
    {
        isOn = false;
        this.GetComponent<SpriteRenderer>().sprite = offSprite;
        this.GetComponentInChildren<SpriteMask>().sprite = offSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onInteracted()
    {
        if(isOn)
        {
            isOn = false;
            this.GetComponent<SpriteRenderer>().sprite = offSprite;
            this.GetComponentInChildren<SpriteMask>().sprite = offSprite;
        }
        else
        {
            isOn = true;
            this.GetComponent<SpriteRenderer>().sprite = onSprite;
            this.GetComponentInChildren<SpriteMask>().sprite = onSprite;
        }

        foreach (Sample_LeverListener aListener in listeners)
        {
            aListener.onLeverStateChanged(isOn);
        }
    }

    public void onBecomeCurrentInteractable()
    {
        this.GetComponentInChildren<SpriteMask>().enabled = true;
    }

    public void onNoLongerCurrentInteractable()
    {
        this.GetComponentInChildren<SpriteMask>().enabled = false;
    }
}
