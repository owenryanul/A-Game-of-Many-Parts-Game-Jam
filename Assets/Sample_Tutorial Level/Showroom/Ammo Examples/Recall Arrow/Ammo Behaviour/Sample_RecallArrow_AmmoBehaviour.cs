using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_RecallArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject recallArrowPrefab;

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
        //No Effect
    }

    public void OnPress(PlayerLogic playerLogic)
    {
        GameObject arrow = Instantiate(recallArrowPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
        arrow.GetComponent<Sample_RecallArrowProjectileScript>().fireInDirection(playerLogic.getAimDirection());
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
    }

    public void OnRelease(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnCancel(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        //No Effect
    }
}
