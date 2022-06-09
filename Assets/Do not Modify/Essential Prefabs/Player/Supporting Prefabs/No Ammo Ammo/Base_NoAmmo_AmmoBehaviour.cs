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

    public void OnFirePressd(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnFireReleased(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnFireCancelled(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        //No Effect
    }
}
