using System.Collections;
using UnityEngine;

public class Flip_BossChargeBall : MonoBehaviour, Flip_ISpawnable
{
    public Renderer BallRenderer;
    public Rigidbody2D BallRigidBody;
    public float TravelTime;
    public Vector2 localStartPosition;
    public Vector2 localEndPosition;
    public Vector2 StartScale;
    public Vector2 TargetScale;

    public Flip_Boss Boss;

    public void Start()
    {
        BallRenderer.enabled = false;
        transform.localScale = StartScale;
    }

    public void Spawn()
    {
        BallRigidBody.simulated = true;
        StartCoroutine(AnimateIn());
    }

    private IEnumerator AnimateIn()
    {
        BallRenderer.enabled = true;
        transform.localScale = StartScale;

        float elapsedTime = 0;

        while (elapsedTime < TravelTime)
        {
            transform.localPosition = Vector2.Lerp(localStartPosition, localEndPosition, elapsedTime / TravelTime);
            transform.localScale = Vector2.Lerp(StartScale, TargetScale, elapsedTime / TravelTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = localEndPosition;
        transform.localScale = TargetScale;
        BallRenderer.enabled = false;

        Boss.transform.localScale += new Vector3(0.03f, 0.03f, 0.03f);
        Destroy(gameObject);
    }
}
