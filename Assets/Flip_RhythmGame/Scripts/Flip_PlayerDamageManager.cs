using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Flip_PlayerDamageManager : MonoBehaviour
{
    public PlayerLogic PlayerLogic;
    public float InvulnerabilityTime;
    public AudioSource Music;
    public float AudioPitchFadeTime;

    public Image FadeImage;
    public float fadeTime;

    private float _timeElapsed;
    private bool _invulnerable;

    public void Update()
    {
        if (!_invulnerable)
        {
            return;
        }

        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= InvulnerabilityTime)
        {
            _invulnerable = false;
            _timeElapsed = 0;
        }
    }

    public void Damage(int damageValue)
    {
        if (_invulnerable)
        {
            return;
        }

        _invulnerable = true;

        if (PlayerLogic.getHP() > 0)
        {
            PlayerLogic.addHp(damageValue, true);
        }

        if (PlayerLogic.getHP() <= 0)
        {
            StartCoroutine(PitchMusic());
            StartCoroutine(FadeAndReset());
        }
    }

    private IEnumerator PitchMusic()
    {
        float timeElapsed = 0;
        float startPitch = Music.pitch;

        while (timeElapsed < AudioPitchFadeTime)
        {
            Music.pitch = Mathf.Lerp(startPitch, 0, timeElapsed / AudioPitchFadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Music.pitch = 0;
        Music.Stop();
    }

    private IEnumerator FadeAndReset()
    {
        float timeElapsed = 0;

        while (timeElapsed < fadeTime)
        {
            FadeImage.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        FadeImage.color = new Color(0, 0, 0, 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
