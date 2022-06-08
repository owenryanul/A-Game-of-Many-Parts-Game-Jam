using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_NoAmmo_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
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
        //No Effect
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
