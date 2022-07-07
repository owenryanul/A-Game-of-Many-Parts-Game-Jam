using System.Collections;
using UnityEngine;

public class Flip_FallingSpike : MonoBehaviour
{
    public Renderer SpikeRenderer;
    public Rigidbody2D SpikeRigidbody;
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public AnimationCurve XPositionAnimationCurve;
    public AnimationCurve YPositionAnimationCurve;

    public void Spawn()
    {
        SpikeRigidbody.simulated = true;
        SpikeRenderer.enabled = true;
        SpikeRigidbody.gravityScale = 0;
        StartCoroutine(SpawnSPike());
    }

    private IEnumerator SpawnSPike()
    {
        float timeElapsed = 0;

        while (timeElapsed < 2)
        {
            var animationCurveValues = new Vector2(XPositionAnimationCurve.Evaluate(timeElapsed), YPositionAnimationCurve.Evaluate(timeElapsed));
            transform.position = new Vector2(Mathf.Lerp(StartPosition.x, EndPosition.x, animationCurveValues.x), Mathf.Lerp(StartPosition.y, EndPosition.y, animationCurveValues.y));

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = EndPosition;
        SpikeRigidbody.gravityScale = 1;
    }
}
