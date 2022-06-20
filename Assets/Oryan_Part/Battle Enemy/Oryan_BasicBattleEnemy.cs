using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Oryan_BasicBattleEnemy : Oryan_TurnTaker, Oryan_ProjectileParent
{
    public GameObject projectile;
    public GameObject parryProjectile;
    public bool doShoot = true;

    public int maxHp;
    private int currentHp;

    public List<Oryan_Element> vulnerablies;
    public List<Oryan_Element> immunities;

    private bool destroyNextUpdate;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        this.gameObject.transform.Find("Sprite").Find("Enemy Canvas").Find("HP Text").GetComponent<TextMeshProUGUI>().text = "" + this.currentHp;
        destroyNextUpdate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(destroyNextUpdate)
        {
            kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public override void onTheirTurnStart()
    {
        Debug.Log("OnTheirTurnStarted " + this.gameObject.name);
        base.onTheirTurnStart();

        if (doShoot)
        {
            if(Random.Range(0, 2) == 0)
            {
                Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
                GameObject bolt = Instantiate(projectile, this.gameObject.transform.position, projectile.transform.rotation);
                bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
                bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayer);
            }
            else
            {
                Vector3 directionTowardsPlayer = GameObject.FindGameObjectWithTag("base_player").transform.position - this.gameObject.transform.position;
                GameObject bolt = Instantiate(parryProjectile, this.gameObject.transform.position, projectile.transform.rotation);
                bolt.GetComponent<Oryan_EnemyProjectile>().setProjectileParent(this);
                bolt.GetComponent<Oryan_EnemyProjectile>().fireInDirection(directionTowardsPlayer);
            }
        }
    }

    public void dealDamage(int damageIn)
    {
        dealDamage(damageIn, new List<Oryan_Element>());
    }

    public void dealDamage(int damageIn, List<Oryan_Element> damageTypes)
    {
        int totalDamage = damageIn;
        bool immunityTriggered = false;
        bool weaknessTriggered = false;
        
        foreach(Oryan_Element aElement in damageTypes)
        {
            if(this.vulnerablies.Contains(aElement))
            {
                totalDamage = totalDamage * 2;
                weaknessTriggered = true;
                break;
            }
            else if(this.immunities.Contains(aElement))
            {
                totalDamage = 0;
                immunityTriggered = true;
            }
        }




        currentHp -= totalDamage;
        if (totalDamage >= 0)
        {
            Debug.Log("name = " + this.gameObject.transform.Find("Sprite").Find("Enemy Canvas").Find("HP Text"));
            this.gameObject.transform.Find("Sprite").Find("Enemy Canvas").Find("HP Text").GetComponent<TextMeshProUGUI>().text = "" + this.currentHp;
            if (immunityTriggered)
            {
                this.gameObject.transform.Find("Sprite").Find("Enemy Canvas").GetComponent<Animator>().SetTrigger("Immune Damaged");
            }
            else if (weaknessTriggered)
            {
                this.gameObject.transform.Find("Sprite").Find("Enemy Canvas").GetComponent<Animator>().SetTrigger("Weak Damaged");
            }
            else
            {
                this.gameObject.transform.Find("Sprite").Find("Enemy Canvas").GetComponent<Animator>().SetTrigger("Normal Damaged");
            }
        }


        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        else if(currentHp <= 0)
        {
            kill();
        }
    }

    private void kill()
    {
            base.removeThemFromTurnTakers();
            Destroy(this.gameObject);
    }

    public void childProjectileDestroyed()
    {
        if(base.isTheirTurn)
        {
            base.endTheirTurn();
        }
    }

    public override void markForDeletion()
    {
        destroyNextUpdate = true;
    }
}
