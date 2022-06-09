using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_BombArrowProjectileLogic : MonoBehaviour, OnActivateListener
{
    public float speed;
    public float facingOffset; //inDegrees
    public GameObject explosionPrefab;

    private Vector3 targetDirection;
    private bool markedForRemoval;

    // Start is called before the first frame update
    void Start()
    {
        markedForRemoval = false;
        GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>().addOnActivateListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(markedForRemoval)
        {
            GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>().removeOnActivateListener(this);
            Destroy(this.gameObject);
        }
    }

    public void fireInDirection(Vector3 targetDirectionIn)
    {
        targetDirection = targetDirectionIn.normalized;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        this.gameObject.GetComponent<Rigidbody2D>().rotation = angle + facingOffset;

        this.gameObject.GetComponent<Rigidbody2D>().velocity = targetDirection * speed;
    }

    public void OnActivatePressed()
    {
        markedForRemoval = true;
        Instantiate(explosionPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
    }

    public void OnActivateReleased()
    {
        //no effect
    }
}
