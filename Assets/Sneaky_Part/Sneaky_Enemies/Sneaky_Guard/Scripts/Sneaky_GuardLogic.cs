using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Sneaky_GuardLogic : MonoBehaviour
{

    public bool isAggroed;
    public float speed = 2;
    public GameObject deathParticleEmitter;
    public GameObject objectToSpawn;
    public bool runTowardsPlayer = false;
    public Transform pathHolder;
    public float waitTime = .3f;
    public float distance;
    public float start_time;
    public bool track_time;

    private float running_time;
    private float alert;

 
    public float backupRadius = 5f;

    public Transform raycaster;
    public LayerMask raycastingMask;
    public float raySize = 60f;

    private GameObject player;

    private bool dying;

    // Start is called before the first frame updateGetComponent<SpriteRenderer>()
    void Start()
    {
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));
        this.player = GameObject.FindWithTag("base_player");
        dying = false;
        start_time = 0;
        running_time = 0;
        
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints [targetWaypointIndex];
    
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!dying)
        {
            
            float rayAngle = raycaster.rotation.y - (raySize / 2f);
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, distance);
            
            for (int ray = 0; ray < raySize; ray++)
            {
                RaycastHit2D hit = Physics2D.Raycast(raycaster.position,
                    Quaternion.AngleAxis(rayAngle, raycaster.forward) * raycaster.up,
                    Mathf.Infinity, raycastingMask);

                if (hit.collider != null)
                {
                    
                    if (hit.transform.CompareTag("base_player"))
                    {

                            
                            isAggroed= true;
                            
                                //Debug.Log("Found Player!");
                            
                    }
                }

                // Debug raycast
                Debug.DrawLine(raycaster.position, hit.point, Color.red);

                rayAngle++;
                
                
            }
            
            
            if (isAggroed && player != null)
            {
                track_time = true;
                //start_time = Time.time;
                running_time = Time.time - start_time;
                if (running_time >= 2)
                {
                    Debug.Log("You're in for it!");
                    Instantiate(objectToSpawn, this.gameObject.transform.position, this.gameObject.transform.rotation);
                    isAggroed = false;
                    ResetTimer();
                }

                
            }
            else if (!isAggroed)
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                track_time = false;
                ResetTimer();

            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "sample_explosion")
        {
            die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "sample_explosion")
        {
            die();
        }
    }

    private void die()
    {
        if (!dying)
        {
            dying = true;
            Instantiate(deathParticleEmitter, this.gameObject.transform.position, this.gameObject.transform.rotation);
            Destroy(this.gameObject);
        }
    }
    
    void onDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPosition);
    }
    void ResetTimer()
    {
        start_time = Time.time;
        
        running_time = 0f;
        //Debug.Log("resetting timer");
    }
}


