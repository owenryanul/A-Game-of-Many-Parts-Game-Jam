using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject chargeArrowPrefab;
    public float maxSpeed;
    public float chargeSpeed;
    public GameObject chargeParticleEmitterPrefab;
    private float currentBuiltUpSpeed;
    private bool charging;
    private GameObject chargeParticleEmitter;

    // Start is called before the first frame update
    void Start()
    {
        currentBuiltUpSpeed = 0;
        charging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(charging)
        {
            currentBuiltUpSpeed += (chargeSpeed * Time.deltaTime);
            if(chargeSpeed > maxSpeed)
            {
                chargeSpeed = maxSpeed;
            }
            chargeParticleEmitter.transform.position = this.transform.position;
            chargeParticleEmitter.transform.localScale = new Vector3(1.0f * (0.5f + currentBuiltUpSpeed / maxSpeed), 1.0f * (0.5f + currentBuiltUpSpeed / maxSpeed), 1);
        }
    }

    public void OnEquip(Player_Logic playerLogic)
    {
        //No Effect
    }

    public void OnPress(Player_Logic playerLogic)
    {
        currentBuiltUpSpeed = 0;
        charging = true;
        chargeParticleEmitter = Instantiate(chargeParticleEmitterPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
    }

    public void OnRelease(Player_Logic playerLogic)
    {
        Debug.Log("Charge Arrow Released");
        GameObject arrow = Instantiate(chargeArrowPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
        arrow.GetComponent<ArrowProjectileLogic>().speed = currentBuiltUpSpeed;
        arrow.GetComponent<ArrowProjectileLogic>().fireInDirection(playerLogic.getAimDirection());
        charging = false;
        Destroy(chargeParticleEmitter);
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
    }

    public void OnCancel(Player_Logic playerLogic)
    {
        Debug.Log("Charge Arrow Canceled");
        if (charging)
        {
            currentBuiltUpSpeed = 0;
            charging = false;
            Destroy(chargeParticleEmitter);
        }
    }

    public void OnUnequip(Player_Logic playerLogic)
    {
        if (chargeParticleEmitterPrefab != null)
        {
            Destroy(chargeParticleEmitter);
        }
        //No Effect
    }
}
