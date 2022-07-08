using UnityEngine;

public class Flip_LevelEnd : MonoBehaviour
{
    [Header("Win Parameters")]
    public Rigidbody2D LevelExit;
    public Vector2 Force;

    [Header("Lose parameters")]
    public Flip_Boss Boss;
    public GameObject Supernova;
    public AudioSource SupernovaAudio;

    private bool _doOnce;

    public void CheckLevelEnd()
    {
        if (_doOnce)
        {
            return;
        }

        _doOnce = true;

        if (Boss == null || Boss.Health <= 0)
        {
            SpawnExit();
        }
        else
        {
            Boss.SetHealth(999);
            Supernova.SetActive(true);
            SupernovaAudio.Play();
        }
    }

    private void SpawnExit()
    {
        LevelExit.AddForce(Force, ForceMode2D.Force);
        Application.runInBackground = false;
    }
}
