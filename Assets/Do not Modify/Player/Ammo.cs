using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ammo
{
    public string name;
    public GameObject prefab;
    public Sprite icon;
    public int quantity;

    public Ammo(string nameIn, GameObject prefabIn, Sprite iconIn, int quantityIn)
    {
        this.name = nameIn;
        this.prefab = prefabIn;
        this.icon = iconIn;
        this.quantity = quantityIn;
    }
}
