using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AmmoBehaviour
{
    //Called when the player presses fire with this ammo equiped
    public abstract void OnPress(PlayerLogic playerLogic);

    //Called when the player releases fire with this ammo equiped
    public abstract void OnRelease(PlayerLogic playerLogic);

    //Called when the player dodges or swaps off this ammo after pressing fire but before releasing it.
    public abstract void OnCancel(PlayerLogic playerLogic);

    //Called when the player first switches to this ammo type
    public abstract void OnEquip(PlayerLogic playerLogic);

    //Called when the switches off this ammo type
    public abstract void OnUnequip(PlayerLogic playerLogic);

}