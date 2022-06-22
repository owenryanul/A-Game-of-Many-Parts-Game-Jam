using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_Shield_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject shieldPropPrefab;
    public Vector3 propOffset;
    public float timeBetweenAmmoLoss;
    private float timeSinceLastAmmoLoss;

    private GameObject heldShieldItem;

    private bool shieldOut;


    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastAmmoLoss = 0;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLogic playerLogic = GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>();
        if (shieldOut)
        {
            timeSinceLastAmmoLoss += Time.deltaTime;
            
            if (timeSinceLastAmmoLoss >= timeBetweenAmmoLoss)
            {
                timeSinceLastAmmoLoss = 0;
                if(playerLogic.getAmmoRelativeToCurrent(0).quantity > 1)
                {
                    playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
                }
                else if(playerLogic.getAmmoRelativeToCurrent(0).quantity == 1)
                {
                    heldShieldItem.transform.Find("Circle").GetComponent<SpriteRenderer>().enabled = false;
                    heldShieldItem.GetComponent<Collider2D>().enabled = false;
                }
            }
        }
    }

    public void OnEquip(PlayerLogic playerLogic)
    {
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().setRechargingShield(false);
        heldShieldItem = Instantiate(shieldPropPrefab, playerLogic.gameObject.transform);
        heldShieldItem.transform.localPosition = propOffset;

        if(playerLogic.getAmmoRelativeToCurrent(0).quantity != 1)
        {
            heldShieldItem.transform.Find("Circle").GetComponent<SpriteRenderer>().enabled = true;
            heldShieldItem.GetComponent<Collider2D>().enabled = true;
            shieldOut = true;
        }     
    }

    public void OnFirePressd(PlayerLogic playerLogic)
    {

    }

    public void OnFireReleased(PlayerLogic playerLogic)
    {
        //no effect
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        shieldOut = false;
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().setRechargingShield(true);
        Destroy(heldShieldItem);
    }

    public void OnFireCancelled(PlayerLogic playerLogic)
    {
        //no effect
    }
}
