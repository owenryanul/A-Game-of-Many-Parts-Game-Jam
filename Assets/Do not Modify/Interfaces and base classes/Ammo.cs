using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ammo
{
    public string name;
    public GameObject ammoBehaviourPrefab;
    public Sprite icon;
    public int quantity;

    public Ammo(string nameIn, GameObject ammoBehaviourPrefabIn, Sprite iconIn, int quantityIn)
    {
        this.name = nameIn;
        this.ammoBehaviourPrefab = ammoBehaviourPrefabIn;
        this.icon = iconIn;
        this.quantity = quantityIn;

        if(this.ammoBehaviourPrefab.GetComponent<AmmoBehaviour>() == null)
        {
            Debug.LogWarning("Warning: The ammoBehaviourPrefab for the ammo named " + this.name + " does not contain a script that implements the AmmoBehaviour interface. This will cause errors if the ammo is used by the player.");
        }
    }
}
