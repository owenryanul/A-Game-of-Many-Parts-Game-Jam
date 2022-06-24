using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_HealingPotion_AmmoBehaviour : MonoBehaviour, AmmoBehaviour, Oryan_ProjectileParent
{
    public GameObject healParticleEffect;
    public GameObject potionItemPrefab;
    public Vector3 potionOffset;

    private GameObject heldPotionItem;


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
        heldPotionItem = Instantiate(potionItemPrefab, playerLogic.gameObject.transform);
        heldPotionItem.transform.localPosition = potionOffset;
    }

    public void OnFirePressd(PlayerLogic playerLogic)
    {
        playerLogic.addHp(1);
        GameObject heal = Instantiate(healParticleEffect, playerLogic.gameObject.transform);
        heal.transform.localPosition = potionOffset;
        heal.GetComponent<Oryan_ProjectileParentNotifier>().setParent(this);
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().lockPlayerQuiverWhileTheirTurnResolves();
    }

    public void OnFireReleased(PlayerLogic playerLogic)
    {
        //no effect
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        Destroy(heldPotionItem);
    }

    public void OnFireCancelled(PlayerLogic playerLogic)
    {
        //no effect
    }

    public void childProjectileDestroyed()
    {
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().endPlayerTurn();
    }

    public void childProjectileDamagedPlayer()
    {
        //no effect
    }

    public void onGrandchildProjectileCreated()
    {
        //no effect
    }

    public bool hasChildProjectilesDamagedPlayerThisAttack()
    {
        //no effect
        return false;
    }
}
