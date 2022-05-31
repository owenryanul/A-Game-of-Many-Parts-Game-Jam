using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_CreditsArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject arrowPrefab;
    private string currentCredits;

    [TextArea]
    public List<string> creditTexts;


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
        //"CreditsArrow_"
        string trimmed = playerLogic.getAmmoRelativeToCurrent(0).name.Remove(0, 13);
        switch (trimmed)
        {
            case "0": currentCredits = creditTexts[0];  break;
            case "1": currentCredits = creditTexts[1]; break;
        }
        
    }

    public void OnPress(PlayerLogic playerLogic)
    {
        GameObject arrow = Instantiate(arrowPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
        arrow.GetComponent<Base_CreditsArrowProjectileLogic>().fireInDirection(playerLogic.getAimDirection(), currentCredits);
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
    }

    public void OnCancel(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnRelease(PlayerLogic playerLogic)
    {
        //No Effect
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        //No Effect
    }
}
