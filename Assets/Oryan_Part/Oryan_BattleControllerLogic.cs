using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnTaker = Oryan_TurnTaker;
using TMPro;

public class Oryan_BattleControllerLogic : TurnTaker, FadeEffectsListener, OnDeathListener
{
    public List<GameObject> enemySpots;
    public Vector3 battleCameraPosition;
    public bool isInBattle;
    private bool endingBattle;
    private bool startingBattle;
    private bool playerHasLostBattle;

    private GameObject player;
    private Vector3 playerOverworldPosition;
    public Vector3 activeCheckpointPosition;
    public GameObject overworldBattleInstigator;

    private List<TurnTaker> turnTakers;
    private int currentTurnIndex;
    public float turnTransitionTime;
    private float turnTransitionTimeRemaining;
    private bool transitioningToNextTurn;

    public float battleEndDelay;
    public float extendedBattleEndDelay;
    private float battleEndDelayRemaining;

    public int maxHp = 3;

    [Header("Quivers")]
    public List<Ammo> attackQuiver;
    public List<Ammo> defenceQuiver;
    public List<Ammo> overworldQuiver;

    [Header("Shield")]
    public Ammo shieldRechargeAmmo;
    public int maxShieldCharge;
    public float timePerShieldRecharge;
    private bool shieldIsRecharging;
    private float timeSinceLastRecharge;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player");
        player.GetComponent<PlayerLogic>().addOnDeathListener(this);
        turnTakers = new List<TurnTaker>();
        turnTakers.Add(this);
        currentTurnIndex = 0;
        isInBattle = false;
        endingBattle = false;
        startingBattle = false;
        playerHasLostBattle = false;

        turnTransitionTimeRemaining = 0;
        transitioningToNextTurn = false;

        GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<ScreenFadeEventsRelay>().addListener(this);

        shieldIsRecharging = false;
        timeSinceLastRecharge = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInBattle && turnTakers.Count < 2)
        {
            battleEndDelayRemaining -= Time.deltaTime;
            if (battleEndDelayRemaining <= 0)
            {
                this.endBattle();
            }
        }

        if(transitioningToNextTurn)
        {
            turnTransitionTimeRemaining -= Time.deltaTime;
            if(turnTransitionTimeRemaining <= 0)
            {
                transitioningToNextTurn = false;
                startNextTurn();
            }
        }

        if(isInBattle && shieldIsRecharging && player.GetComponent<PlayerLogic>().getAmmoFromName(shieldRechargeAmmo.name) != null)
        {
            if (player.GetComponent<PlayerLogic>().getAmmoFromName(shieldRechargeAmmo.name).quantity < maxShieldCharge)
            {
                timeSinceLastRecharge += Time.deltaTime;
                if (timeSinceLastRecharge >= timePerShieldRecharge)
                {
                    player.GetComponent<PlayerLogic>().addAmmo(shieldRechargeAmmo);
                    timeSinceLastRecharge = 0;
                }
            }
        }
    }

    public void startBattle(GameObject overworldInstigatorIn, GameObject[] enemies, bool extendFightEnding = false)
    {
        isInBattle = true;
        startingBattle = true;
        playerHasLostBattle = false;
        currentTurnIndex = 0;
        overworldBattleInstigator = overworldInstigatorIn;
        battleEndDelayRemaining = battleEndDelay;
        if(extendFightEnding)
        {
            battleEndDelayRemaining = extendedBattleEndDelay;
        }
        
        //Spawn Enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            if (i >= enemySpots.Count)
            {
                Debug.LogWarning("Too many enemies in this encounter, insufficent enemy spots.");
                break;
            }
            else if(enemies[i] != null)
            {
                addEnemyToBattle(enemies[i], i);
            }
        }

        GameObject.FindGameObjectWithTag("oryan_turnText").GetComponent<TextMeshProUGUI>().text = "Player's Turn";
        GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<Animator>().SetTrigger("FadeOut");
    }

    private void startBattleAfterFadeOut()
    {
        playerOverworldPosition = player.transform.position;
        player.transform.position = GameObject.FindGameObjectWithTag("oryan_playerCage").transform.position;
        //player.GetComponent<Rigidbody2D>().MovePosition(GameObject.FindGameObjectWithTag("oryan_playerCage").transform.position);
        player.GetComponent<PlayerLogic>().playerCameraFollowsPlayer = false;
        player.GetComponent<PlayerLogic>().playerCamera.transform.position = battleCameraPosition;
        player.GetComponent<PlayerLogic>().playerCamera.gameObject.GetComponentInChildren<Light>().enabled = false;

        //Empty Quiver and replace with defence quiver
        overworldQuiver = copyAmmoListByValue(player.GetComponent<PlayerLogic>().carriedAmmo);
        emptyPlayerAmmo();

        //Set turn to player's turn
        currentTurnIndex = 0;
        this.onTheirTurnStart();

        GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<Animator>().SetTrigger("FadeIn");
    }

    public void endBattle()
    {
        isInBattle = false;
        currentTurnIndex = 0;
        endingBattle = true;
        GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<Animator>().SetTrigger("FadeOut");

        if(!playerHasLostBattle)
        {
            //If player wins, destroy the overworld enemy that started the battle.
            Destroy(overworldBattleInstigator);
        }
        else
        {
            //Remove any enemies that are left after the player's defeat
            for(int i = 1; i < turnTakers.Count; i++)
            {
                turnTakers[i].markForDeletion();
            }
        }

        GameObject.FindGameObjectWithTag("oryan_turnText").GetComponent<TextMeshProUGUI>().text = "";
    }

    public GameObject addEnemyToBattle(GameObject enemyPrefab, int pos)
    {
        GameObject enemy = Instantiate(enemyPrefab, enemySpots[pos].transform.position, enemyPrefab.transform.rotation);
        enemy.GetComponent<Oryan_BasicBattleEnemy>().setBattlePosition(pos);
        turnTakers.Add(enemy.GetComponent<TurnTaker>());
        return enemy;
    }


    public void OnFadeOutDone()
    {
        if(endingBattle)
        {
            endingBattle = false;
            if (playerHasLostBattle)
            {
                player.transform.position = activeCheckpointPosition;
                player.GetComponent<PlayerLogic>().respawn();
                player.GetComponent<PlayerLogic>().setHp(this.maxHp);

            }
            else
            {
                player.transform.position = playerOverworldPosition;
            }
            player.GetComponent<PlayerLogic>().playerCameraFollowsPlayer = true;
            player.GetComponent<PlayerLogic>().playerCamera.gameObject.GetComponentInChildren<Light>().enabled = true;

            emptyPlayerAmmo();
            fillCarriedAmmoWith(overworldQuiver);
            GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<Animator>().SetTrigger("FadeIn");
        }
        
        if(startingBattle)
        {
            startingBattle = false;
            startBattleAfterFadeOut();
        }
    }

    public void OnFadeInDone()
    {
        //No effect
    }

    public void nextTurn()
    {
        transitioningToNextTurn = true;
        turnTransitionTimeRemaining = turnTransitionTime;
        GameObject.FindGameObjectWithTag("oryan_turnText").GetComponent<Animator>().SetTrigger("Switch");
    }

    private void startNextTurn()
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
        if (isInBattle)
        {
            startPlayerTurn();
        }
    }

    public override void endTheirTurn()
    {
        base.endTheirTurn();
    }

    private void emptyPlayerAmmo()
    {
        while (player.GetComponent<PlayerLogic>().carriedAmmo.Count > 0)
        {
            player.GetComponent<PlayerLogic>().modifyAmmoAmount(player.GetComponent<PlayerLogic>().getAmmoRelativeToCurrent(0), -player.GetComponent<PlayerLogic>().getAmmoRelativeToCurrent(0).quantity);
        }
    }


    private void fillCarriedAmmoWith(List<Ammo> ammos)
    {
        for (int i = 0; i < ammos.Count; i++)
        {
            Debug.Log("Adding Ammo: " + ammos[i].name);
            Ammo ammoToAdd = new Ammo(ammos[i].name, ammos[i].ammoBehaviourPrefab, ammos[i].icon, ammos[i].quantity);
            player.GetComponent<PlayerLogic>().addAmmo(ammoToAdd);
        }
    }

    //Do not use with CarriedAmmo, as it won't trigger .addAmmo and will cause issues with the ammo indicator and currently equiped ammo type.
    private List<Ammo> copyAmmoListByValue(List<Ammo> ammos)
    {
        List<Ammo> newList = new List<Ammo>();
        for (int i = 0; i < ammos.Count; i++)
        {
            Debug.Log("Adding Ammo: " + ammos[i].name);
            Ammo ammoToAdd = new Ammo(ammos[i].name, ammos[i].ammoBehaviourPrefab, ammos[i].icon, ammos[i].quantity);
            newList.Add(ammoToAdd);
        }
        return newList;
    }

    private void startPlayerTurn()
    {
        emptyPlayerAmmo();
        fillCarriedAmmoWith(attackQuiver);
    }

    public void endPlayerTurn()
    {
        fillCarriedAmmoWith(defenceQuiver);

        this.nextTurn();
    }

    public void lockPlayerQuiverWhileTheirTurnResolves()
    {
        attackQuiver = copyAmmoListByValue(player.GetComponent<PlayerLogic>().carriedAmmo);
        emptyPlayerAmmo();
    }

    public void OnPlayerDies(PlayerLogic playerLogic)
    {
        playerHasLostBattle = true;
        endBattle();
    }

    public GameObject getCurrentTurnTaker()
    {
        return turnTakers[currentTurnIndex].gameObject;
    }

    public GameObject getNextTurnTaker()
    {
        if(currentTurnIndex + 1 >= turnTakers.Count)
        {
            return turnTakers[0].gameObject;      
        }
        else
        {
            return turnTakers[currentTurnIndex + 1].gameObject;
        }
    }

    public void setRechargingShield(bool rechargeIn)
    {
        this.shieldIsRecharging = rechargeIn;
    }

}
