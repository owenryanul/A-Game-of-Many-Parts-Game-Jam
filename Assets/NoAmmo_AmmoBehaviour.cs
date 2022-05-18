using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAmmo_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
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
        //No Effect
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
