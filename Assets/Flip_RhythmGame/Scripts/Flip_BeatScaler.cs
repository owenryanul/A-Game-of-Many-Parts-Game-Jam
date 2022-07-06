using Flip_SynchronizerData;
using UnityEngine;

public class Flip_BeatScaler : MonoBehaviour
{
    public Flip_BeatObserver BeatObserver;
    public BeatType BeatType;
    public Transform[] TargetsToScale;
    public Vector3 NeutralScale;
    public Vector3 BeatScale;
    public float TimeToResetScale;

    private float _elapsedTime;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if ((BeatObserver.beatMask & BeatType) == BeatType)
        {
            foreach (Transform target in TargetsToScale)
            {
                target.localScale = BeatScale;
            }

            _elapsedTime = 0;
        }
        else
        {
            foreach (Transform target in TargetsToScale)
            {
                target.localScale = Vector3.Lerp(BeatScale, NeutralScale, _elapsedTime / TimeToResetScale);
            }

        }
    }
}
