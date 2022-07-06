using System.Collections;
using UnityEngine;

public class Flip_MovingBlock : MonoBehaviour, Flip_ISpawnable
{
    public Renderer BlockRenderer;
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public float TimeToMove;

    private const float spawnDelay = 2;

    public void Start()
    {
        BlockRenderer.enabled = false;
    }

    public void Spawn()
    {
        StartCoroutine(MoveBlock());
    }

    private IEnumerator MoveBlock()
    {
        float timeElapsed = 0;

        while (timeElapsed < spawnDelay)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed -= spawnDelay;

        BlockRenderer.enabled = true;

        while (timeElapsed < TimeToMove)
        {
            transform.position = Vector2.Lerp(StartPosition, EndPosition, timeElapsed / TimeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = EndPosition;
        BlockRenderer.enabled = false;
    }
}
