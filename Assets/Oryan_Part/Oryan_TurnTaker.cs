using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Oryan_TurnTaker : MonoBehaviour
{
    protected bool isTheirTurn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void onTheirTurnStart()
    {
        this.isTheirTurn = true;
    }

    public virtual void endTheirTurn()
    {
        this.isTheirTurn = false;
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().nextTurn();
    }

    public virtual void removeThemFromTurnTakers()
    {
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().removeTurnTaker(this);
        if(this.isTheirTurn)
        {
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().nextTurn();
        }
    }

    public virtual void markForDeletion()
    {
        
    }
}
