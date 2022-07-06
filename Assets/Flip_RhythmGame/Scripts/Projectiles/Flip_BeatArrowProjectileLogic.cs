using Flip_SynchronizerData;
using UnityEngine;

public class Flip_BeatArrowProjectileLogic : MonoBehaviour, OnActivateListener
{
    public Flip_BeatObserver ArrowBeatExplosionObserver;
    public BeatType BeatType;
    private bool _waitingForNextBeat = true;


    public float speed;
    public float facingOffset; //inDegrees
    public GameObject explosionPrefab;

    private Vector3 targetDirection;
    private bool markedForRemoval;

    private void Start()
    {
        markedForRemoval = false;
        GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>().addOnActivateListener(this);
    }

    private void Update()
    {
        if ((ArrowBeatExplosionObserver.beatMask & BeatType) != BeatType)
        {
            _waitingForNextBeat = false;
        }

        if (_waitingForNextBeat == false && (ArrowBeatExplosionObserver.beatMask & BeatType) == BeatType)
        {
            markedForRemoval = true;
            Instantiate(explosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
        }

        if(markedForRemoval)
        {
            GameObject.FindGameObjectWithTag("base_player").GetComponent<PlayerLogic>().removeOnActivateListener(this);
            Destroy(gameObject);
        }
    }

    public void fireInDirection(Vector3 targetDirectionIn)
    {
        targetDirection = targetDirectionIn.normalized;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        gameObject.GetComponent<Rigidbody2D>().rotation = angle + facingOffset;

        gameObject.GetComponent<Rigidbody2D>().velocity = targetDirection * speed;
    }

    public void OnActivatePressed() { }

    public void OnActivateReleased() { }
}
