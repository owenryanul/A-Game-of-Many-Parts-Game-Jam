using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_BasicAttackArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour, Oryan_ProjectileParent
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

    public void OnEquip(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnFirePressd(PlayerLogic playerLogic)
    {
        GameObject arrow = Instantiate(arrowPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
        arrow.GetComponent<Oryan_BattleArrowProjectileLogic>().fireInDirection(playerLogic.getAimDirection());
        arrow.GetComponent<Oryan_BattleArrowProjectileLogic>().setProjectileParent(this);
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);

        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().lockPlayerQuiverWhileTheirTurnResolves();
    }

    public void OnFireCancelled(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnFireReleased(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void childProjectileDestroyed()
    {
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().endPlayerTurn();
    }
}
