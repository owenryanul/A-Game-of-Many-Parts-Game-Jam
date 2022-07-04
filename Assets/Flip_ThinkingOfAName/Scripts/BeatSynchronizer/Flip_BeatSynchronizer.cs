using UnityEngine;

/// <summary>
/// responsible for synching up the beginning of the audio clip with all active beat counters and pattern counters.
/// </summary>
public class Flip_BeatSynchronizer : MonoBehaviour
{
    public AudioSource AudioSource;
    public float Bpm = 120f;         // Tempo in beats per minute of the audio clip.
    public float StartDelay = 1f;    // Number of seconds to delay the start of audio playback.
    public delegate void AudioStartAction(double syncTime);
    public static event AudioStartAction OnAudioStart;

    void Start ()
    {
        double initTime = AudioSettings.dspTime;
        AudioSource.PlayScheduled(initTime + StartDelay);
        if (OnAudioStart != null)
        {
            OnAudioStart(initTime + StartDelay);
        }
    }

}
