using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AmmoBehaviour
{
    public abstract void OnPress(Player_Logic playerLogic);

    public abstract void OnRelease(Player_Logic playerLogic);

    public abstract void OnEquip(Player_Logic playerLogic);

    public abstract void OnUnequip(Player_Logic playerLogic);
}
