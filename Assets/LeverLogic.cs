using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverLogic : MonoBehaviour, Interactable
{
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
