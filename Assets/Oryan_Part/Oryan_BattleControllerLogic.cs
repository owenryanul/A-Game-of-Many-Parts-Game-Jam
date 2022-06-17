using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnTaker = Oryan_TurnTaker;

public class Oryan_BattleControllerLogic : TurnTaker
{
    public List<Vector3> enemySpots;
    public Vector3 battleCameraPosition;
    public bool isInBattle;

    private GameObject player;
    private Vector3 playerOverworldPosition;

    private List<TurnTaker> turnTakers;
    private int currentTurnIndex;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player");
        turnTakers = new List<TurnTaker>();
        turnTakers.Add(this);
        currentTurnIndex = 0;
        isInBattle = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInBattle && player.GetComponent<PlayerLogic>().getIsDodging())
        {
            this.nextTurn();
        }

        if(isInBattle && turnTakers.Count < 2)
        {
            this.endBattle();
        }
    }

    public void startBattle(List<GameObject> enemies)
    {
        isInBattle = true;
        playerOverworldPosition = player.transform.position;
        player.transform.position = GameObject.FindGameObjectWithTag("oryan_playerCage").transform.position;
        //player.GetComponent<Rigidbody2D>().MovePosition(GameObject.FindGameObjectWithTag("oryan_playerCage").transform.position);
        player.GetComponent<PlayerLogic>().playerCameraFollowsPlayer = false;
        player.GetComponent<PlayerLogic>().playerCamera.transform.position = battleCameraPosition;


        for(int i = 0; i < enemies.Count; i++)
        {
            if (i >= enemySpots.Count)
            {
                Debug.LogWarning("Too many enemies in this encounter, insufficent enemy spots.");
                break;
            }
            else
            {
                GameObject enemy = Instantiate(enemies[i], enemySpots[i], enemies[i].transform.rotation);
                turnTakers.Add(enemy.GetComponent<TurnTaker>());
            }
        }

        currentTurnIndex = 0;
    }

    public void endBattle()
    {
        isInBattle = false;
        player.transform.position = playerOverworldPosition;
        //player.GetComponent<Rigidbody2D>().MovePosition(playerOverworldPosition);
        player.GetComponent<PlayerLogic>().playerCameraFollowsPlayer = true;
    }

    public void nextTurn()
    {
        currentTurnIndex++;
        if(currentTurnIndex >= turnTakers.Count)
        {
            currentTurnIndex = 0;
        }
        turnTakers[currentTurnIndex].onTheirTurnStart();
    }

    public void removeTurnTaker(TurnTaker turnTakerToRemove)
    {
        int targetIndex = -1;
        for(int i = 0; i < turnTakers.Count; i++)
        {
            if(turnTakers[i] == turnTakerToRemove)
            {
                targetIndex = i;
                break;
            }
        }

        if(targetIndex == -1)
        {
            Debug.LogError("Error: Could not find turnTakerToRemove in list of active turnTakers");
            return;
        }
        
        if(targetIndex < currentTurnIndex)
        {
            currentTurnIndex--;
        }
        turnTakers.RemoveAt(targetIndex);
    }

    public override void onTheirTurnStart()
    {
        base.onTheirTurnStart();
    }

    public override void endTheirTurn()
    {
        base.endTheirTurn();
    }
}
