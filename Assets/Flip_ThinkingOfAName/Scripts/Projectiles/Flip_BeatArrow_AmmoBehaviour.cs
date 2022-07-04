using UnityEngine;

public class Flip_BeatArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
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
        arrow.GetComponent<Flip_BeatArrowProjectileLogic>().fireInDirection(playerLogic.getAimDirection());
        playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
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
}
