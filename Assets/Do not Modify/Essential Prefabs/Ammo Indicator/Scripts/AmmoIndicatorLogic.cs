using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoIndicatorLogic : MonoBehaviour
{
    private PlayerLogic player;
    private Image currentAmmoImage;
    private TextMeshProUGUI currentAmmoCount;
    private Image nextAmmoImage;
    private TextMeshProUGUI nextAmmoCount;
    private Image previousAmmoImage;
    private TextMeshProUGUI previousAmmoCount;
    private Image nextNextAmmoImage;
    private TextMeshProUGUI nextNextAmmoCount;
    private Image previousPreviousAmmoImage;
    private TextMeshProUGUI previousPreviousAmmoCount;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>();
        currentAmmoImage = GameObject.FindGameObjectWithTag("base_currentAmmoImage").GetComponent<Image>();
        currentAmmoCount = GameObject.FindGameObjectWithTag("base_currentAmmoImage").GetComponentInChildren<TextMeshProUGUI>();
        nextAmmoImage = GameObject.FindGameObjectWithTag("base_nextAmmoImage").GetComponent<Image>();
        nextAmmoCount = GameObject.FindGameObjectWithTag("base_nextAmmoImage").GetComponentInChildren<TextMeshProUGUI>();
        nextNextAmmoImage = GameObject.FindGameObjectWithTag("base_nextNextAmmoImage").GetComponent<Image>();
        nextNextAmmoCount = GameObject.FindGameObjectWithTag("base_nextNextAmmoImage").GetComponentInChildren<TextMeshProUGUI>();
        previousAmmoImage = GameObject.FindGameObjectWithTag("base_previousAmmoImage").GetComponent<Image>();
        previousAmmoCount = GameObject.FindGameObjectWithTag("base_previousAmmoImage").GetComponentInChildren<TextMeshProUGUI>();
        previousPreviousAmmoImage = GameObject.FindGameObjectWithTag("base_previousPreviousAmmoImage").GetComponent<Image>();
        previousPreviousAmmoCount =   GameObject.FindGameObjectWithTag("base_previousPreviousAmmoImage").GetComponentInChildren<TextMeshProUGUI>();

        setAmmoIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAmmoIndicator()
    {
        currentAmmoImage.sprite = player.getAmmoRelativeToCurrent(0).icon;
        currentAmmoCount.text = "" + player.getAmmoRelativeToCurrent(0).quantity;
        nextAmmoImage.sprite = player.getAmmoRelativeToCurrent(1).icon;
        nextAmmoCount.text = "" + player.getAmmoRelativeToCurrent(1).quantity;
        nextNextAmmoImage.sprite = player.getAmmoRelativeToCurrent(2).icon;
        nextNextAmmoCount.text = "" + player.getAmmoRelativeToCurrent(2).quantity;
        previousAmmoImage.sprite = player.getAmmoRelativeToCurrent(-1).icon;
        previousAmmoCount.text = "" + player.getAmmoRelativeToCurrent(-1).quantity;
        previousPreviousAmmoImage.sprite = player.getAmmoRelativeToCurrent(-2).icon;
        previousPreviousAmmoCount.text = "" + player.getAmmoRelativeToCurrent(-2).quantity;
        player.isDoneChangingAmmo();
    }
}
