using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
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

    public void OnEquip(Player_Logic playerLogic)
    {
        //No Effect
    }

    public void OnPress(Player_Logic playerLogic)
    {
        GameObject arrow = Instantiate(recallArrowPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
        arrow.GetComponent<RecallArrowProjectileScript>().fireInDirection(playerLogic.getAimDirection());
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
    }

    public void OnRelease(Player_Logic playerLogic)
    {
        //No Effect
    }

    public void OnCancel(Player_Logic playerLogic)
    {
        //No Effect
    }

    public void OnUnequip(Player_Logic playerLogic)
    {
        //No Effect
    }
}
