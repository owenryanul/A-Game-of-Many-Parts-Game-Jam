using System.Collections;
using UnityEngine;

public class Flip_Block : MonoBehaviour, Flip_ISpawnable
{
    public Rigidbody2D BlockRigidBody;
    public Renderer BlockRenderer;
    public Vector2 StartScale;
    public Vector2 TargetScale;

    public Color StartColor;
    public Color EndColor;
    public Color HarmfulColor;
    public Color HarmfulExtendedColor;
    public float AnimationInTime;
    public float HarmfulTime;
    public float HarmfulExtendedTime;
    public float AnimateOutTime;

    public void Start()
    {
        BlockRenderer.enabled = false;
    }

    public void Spawn()
    {
        StartCoroutine(AnimateBlockIn());
    }

    private IEnumerator AnimateBlockIn()
    {
        transform.localScale = StartScale;
        BlockRenderer.material.color = StartColor;
        BlockRenderer.enabled = true;

        float timeElapsed = 0;

        while (timeElapsed < AnimationInTime)
        {
            transform.localScale = Vector2.Lerp(StartScale, TargetScale, timeElapsed / AnimationInTime);
            BlockRenderer.material.color = Color.Lerp(StartColor, EndColor, timeElapsed / AnimationInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = TargetScale;
        BlockRenderer.material.color = HarmfulColor;

        StartCoroutine(MakeBlockHarmful());
    }

    private IEnumerator MakeBlockHarmful()
    {
        float timeElapsed = 0;
        BlockRigidBody.simulated = true;
        Flip_CameraShake.Shake(HarmfulTime, 0.05f);

        while (timeElapsed < HarmfulTime)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0;

        while (timeElapsed < HarmfulExtendedTime)
        {
            BlockRenderer.material.color = Color.Lerp(HarmfulColor, HarmfulExtendedColor, timeElapsed / HarmfulTime);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        BlockRigidBody.simulated = false;
        StartCoroutine(AnimateBlockOut());
    }

    private IEnumerator AnimateBlockOut()
    {
        float timeElapsed = 0;

        while (timeElapsed < AnimateOutTime)
        {
            transform.localScale = Vector2.Lerp(TargetScale, StartScale, timeElapsed / AnimateOutTime);
            BlockRenderer.material.color = Color.Lerp(HarmfulColor, StartColor, timeElapsed / AnimateOutTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = StartScale;
        BlockRenderer.material.color = StartColor;
        BlockRenderer.enabled = false;
    }
}
