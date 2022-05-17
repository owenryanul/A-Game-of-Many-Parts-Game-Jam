using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoIndicatorLogic : MonoBehaviour
{
    private Player_Logic player;
    private Image currentAmmoImage;
    private Image nextAmmoImage;
    private Image previousAmmoImage;
    private Image nextNextAmmoImage;
    private Image previousPreviousAmmoImage;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player").GetComponent<Player_Logic>();
        currentAmmoImage = GameObject.FindGameObjectWithTag("sample_currentAmmoImage").GetComponent<Image>();
        nextAmmoImage = GameObject.FindGameObjectWithTag("sample_nextAmmoImage").GetComponent<Image>();
        nextNextAmmoImage = GameObject.FindGameObjectWithTag("sample_nextNextAmmoImage").GetComponent<Image>();
        previousAmmoImage = GameObject.FindGameObjectWithTag("sample_previousAmmoImage").GetComponent<Image>();
        previousPreviousAmmoImage = GameObject.FindGameObjectWithTag("sample_previousPreviousAmmoImage").GetComponent<Image>();

        setAmmoIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAmmoIndicator()
    {
        currentAmmoImage.sprite = player.getAmmoRelativeToCurrent(0).icon;
        nextAmmoImage.sprite = player.getAmmoRelativeToCurrent(1).icon;
        nextNextAmmoImage.sprite = player.getAmmoRelativeToCurrent(2).icon;
        previousAmmoImage.sprite = player.getAmmoRelativeToCurrent(-1).icon;
        previousPreviousAmmoImage.sprite = player.getAmmoRelativeToCurrent(-2).icon;
        player.changingAmmo = false;
    }
}
