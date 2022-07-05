using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Oryan_TurnTextEventRelay : MonoBehaviour
{
    private Oryan_BattleControllerLogic battleController;

    // Start is called before the first frame update
    void Start()
    {
        battleController = GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnOffScreen()
    {
        if (battleController.getNextTurnTaker().gameObject == battleController.gameObject)
        {
            this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Player's Turn";
        }
        else
        {
            this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = battleController.getNextTurnTaker().screenName + "'s Turn";
        }
    }
}
