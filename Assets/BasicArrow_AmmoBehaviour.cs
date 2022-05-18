using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject arrowPrefab;

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
        Instantiate(arrowPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
    }

    public void OnRelease(Player_Logic playerLogic)
    {
        //No Effect
    }

    public void OnUnequip(Player_Logic playerLogic)
    {
        //No Effect
    }
}
