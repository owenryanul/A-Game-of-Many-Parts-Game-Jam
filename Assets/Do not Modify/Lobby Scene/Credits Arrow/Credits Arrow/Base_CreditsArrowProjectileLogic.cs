using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Base_CreditsArrowProjectileLogic : MonoBehaviour
{
    public float speed;
    public float facingOffset; //inDegrees
    public float minVelocityBeforePickupable;
    public GameObject creditsTextPrefab;
                               
    private Vector3 targetDirection;

    private string credits;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude <= minVelocityBeforePickupable)
        {
            GameObject text = Instantiate(creditsTextPrefab, this.gameObject.transform.position, creditsTextPrefab.transform.rotation);
            text.GetComponentInChildren<TextMeshProUGUI>().text = this.credits;
            Destroy(this.gameObject);
        }
    }

    public void fireInDirection(Vector3 targetDirectionIn, string creditsTextIn)
    {
        targetDirection = targetDirectionIn.normalized;
        credits = creditsTextIn;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        this.gameObject.GetComponent<Rigidbody2D>().rotation = angle + facingOffset;

        this.gameObject.GetComponent<Rigidbody2D>().velocity = targetDirection * speed;
    }

}
