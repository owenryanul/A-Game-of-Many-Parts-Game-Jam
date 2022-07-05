using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_Torch_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject torchPropPrefab;
    public Vector3 propOffset;
    private Vector3 mirrorOffset;
    public Sprite litTorch;
    public Sprite unLitTorch;

    private GameObject heldTorchItem;

    private bool lit;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player");
        mirrorOffset = new Vector3(-propOffset.x, propOffset.y, propOffset.z);
    }

    // Update is called once per frame
    void Update()
    {

        if(heldTorchItem.GetComponentInChildren<SpriteRenderer>().flipX)
        {
            heldTorchItem.transform.localPosition = mirrorOffset;
        }
        else
        {
            heldTorchItem.transform.localPosition = propOffset;
        }
    }

    public void OnEquip(PlayerLogic playerLogic)
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Light>().enabled = false;

        heldTorchItem = Instantiate(torchPropPrefab, this.gameObject.transform);

        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_GlobalDataHolder>().lightSourcesLightingThePlayer++;
        lit = true;
    }

    public void OnFireCancelled(PlayerLogic playerLogic)
    {

    }

    public void OnFirePressd(PlayerLogic playerLogic)
    {
        if(lit)
        {
            lit = false;
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_GlobalDataHolder>().lightSourcesLightingThePlayer--;
            heldTorchItem.GetComponentInChildren<SpriteRenderer>().sprite = unLitTorch;
            this.gameObject.GetComponentInChildren<Light>().enabled = false;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Light>().enabled = true;
        }
        else
        {
            lit = true;
            GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_GlobalDataHolder>().lightSourcesLightingThePlayer++;
            heldTorchItem.GetComponentInChildren<SpriteRenderer>().sprite = litTorch;
            this.gameObject.GetComponentInChildren<Light>().enabled = true;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Light>().enabled = false;
        }
        
    }

    public void OnFireReleased(PlayerLogic playerLogic)
    {
        
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        Destroy(heldTorchItem);
        GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_GlobalDataHolder>().lightSourcesLightingThePlayer--;
        if (!GameObject.FindGameObjectWithTag("oryan_battleController").GetComponent<Oryan_BattleControllerLogic>().isInBattle)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Light>().enabled = true;
        }
    }
}
