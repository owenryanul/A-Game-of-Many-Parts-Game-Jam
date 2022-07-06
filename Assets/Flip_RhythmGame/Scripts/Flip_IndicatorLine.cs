using System.Collections;
using UnityEngine;

public class Flip_IndicatorLine : MonoBehaviour, Flip_ISpawnable
{
    public Renderer IndicatorRenderer;
    public Vector2 StartScale;
    public Vector2 TargetScale;

    public Color StartColor;
    public Color EndColor;
    public float AnimationInTime;

    public float StartingRotation;
    public float RotationSpeed;

    private const float spawnDelay = 1.9f;

    public void Start()
    {
        IndicatorRenderer.enabled = false;
        transform.rotation = Quaternion.Euler(0, 0, StartingRotation);
    }

    public void Update()
    {
        transform.Rotate(Vector3.forward * (RotationSpeed * Time.deltaTime));
    }

    public void Spawn()
    {
        StartCoroutine(AnimateIn());
    }

    private IEnumerator AnimateIn()
    {
        float timeElapsed = 0;

        while (timeElapsed < spawnDelay)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed -= spawnDelay;

        IndicatorRenderer.transform.localScale = StartScale;
        IndicatorRenderer.material.color = StartColor;
        IndicatorRenderer.enabled = true;

        while (timeElapsed < AnimationInTime)
        {
            IndicatorRenderer.transform.localScale = Vector2.Lerp(StartScale, TargetScale, timeElapsed / AnimationInTime);
            IndicatorRenderer.material.color = Color.Lerp(StartColor, EndColor, timeElapsed / AnimationInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        IndicatorRenderer.transform.localScale = TargetScale;
    }
}
