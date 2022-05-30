using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_ArrowProjectileLogic : MonoBehaviour
{
    public float speed;
    public float facingOffset; //inDegrees
    public float minVelocityBeforePickupable;
    public GameObject retrievableArrow;
                               
    private Vector3 targetDirection;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude <= minVelocityBeforePickupable)
        {
            Instantiate(retrievableArrow, this.gameObject.transform.position, this.gameObject.transform.rotation);
            Destroy(this.gameObject);
        }
    }

    public void fireInDirection(Vector3 targetDirectionIn)
    {
        targetDirection = targetDirectionIn;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        this.gameObject.GetComponent<Rigidbody2D>().rotation = angle + facingOffset;

        this.gameObject.GetComponent<Rigidbody2D>().velocity = targetDirection * speed;
    }
}
