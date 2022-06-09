using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_Potion_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject healParticleEffect;
    public GameObject potionItemPrefab;
    public Vector3 potionOffset;

    private GameObject heldPotionItem;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEquip(PlayerLogic playerLogic)
    {
        heldPotionItem = Instantiate(potionItemPrefab, playerLogic.gameObject.transform);
        heldPotionItem.transform.localPosition = potionOffset;
    }

    public void OnFirePressd(PlayerLogic playerLogic)
    {
        playerLogic.addHp(1);
        GameObject heal = Instantiate(healParticleEffect, playerLogic.gameObject.transform);
        heal.transform.localPosition = potionOffset;
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
    }

    public void OnFireReleased(PlayerLogic playerLogic)
    {
        //no effect
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        Destroy(heldPotionItem);
    }

    public void OnFireCancelled(PlayerLogic playerLogic)
    {
        //no effect
    }

}
