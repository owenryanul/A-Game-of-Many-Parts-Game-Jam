using System.Collections;
using UnityEngine;

public class Flip_CameraShake : MonoBehaviour
{
    public static Flip_CameraShake Instance;

    public Transform Camera;
    private Vector3 _startPosition;

    void Awake()
    {
        Instance = this;
        _startPosition = Camera.position;
    }

    public static void Shake(float duration, float amount)
    {
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.ShakeCamera(duration, amount));
    }

    private IEnumerator ShakeCamera(float duration, float amount)
    {
        while (duration > 0)
        {
            Camera.position = _startPosition + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime;
            yield return null;
        }

        Camera.position = _startPosition;
    }
}
