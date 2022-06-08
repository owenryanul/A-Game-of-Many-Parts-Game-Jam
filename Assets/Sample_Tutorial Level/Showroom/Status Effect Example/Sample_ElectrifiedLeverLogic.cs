using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_ElectrifiedLeverLogic : MonoBehaviour, Interactable
{
    public Sprite onSprite;
    public Sprite offSprite;
    public GameObject electricShockParticlePrefab;
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
        GameObject player = GameObject.FindGameObjectWithTag("base_player");
        if (player.GetComponent<PlayerLogic>().getStatus("Wet") != null)
        {
            Instantiate(electricShockParticlePrefab, player.transform.position, player.transform.rotation);
            player.GetComponent<PlayerLogic>().addHp(-10, true);
        }
        else
        {
            if (isOn)
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
