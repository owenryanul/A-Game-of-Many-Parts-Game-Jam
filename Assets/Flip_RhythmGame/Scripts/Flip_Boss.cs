using System.Collections;
using Flip_SynchronizerData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Flip_Boss : MonoBehaviour, Flip_ISpawnable
{
    [Header("References")]
    public Rigidbody2D BossRigidBody;
    public Renderer BossRenderer;
    public Flip_PlayerDamageManager Player;

    [Header("Spawn Parameters")]
    public Vector2 StartScale;
    public Vector2 TargetScale;
    public Color StartColor;
    public Color FullWarningColor;
    public Color EndColor;
    public Color HarmfulColor;
    public float AnimationInTime;
    public float HarmfulTime;
    public float AnimateOutTime;

    [Header("Fight Start")]
    public Image ScreenFadeImage;
    public float ScreenFlashDuration;
    public GameObject HealthIndicator;
    public Flip_BeatObserver BeatObserver;
    public BeatType BeatType;

    [Header("Boss Parameters")]
    public float Health;
    public TMP_Text HealthText;
    public GameObject Bullet;
    public float ShootRotationSpeed;
    public Vector2 BulletForce;
    public GameObject Explosion;
    public Vector2 ExplosionMinPosition;
    public Vector2 ExplosionMaxPosition;

    private bool _spawned;
    private bool _bossFightStarted;
    private bool _shootOnCooldown;
    private Quaternion _bulletRotation;

    private const float spawnDelay = 1;
    private const float fightStartDelay = 2;

    public void Start()
    {
        BossRenderer.enabled = false;
        BossRigidBody.simulated = false;
        HealthText.text = Health.ToString();
    }

    public void Update()
    {
        _bulletRotation.eulerAngles += new Vector3(0, 0, ShootRotationSpeed * Time.deltaTime);

        if (!_bossFightStarted)
        {
            return;
        }

        if ((BeatObserver.beatMask & BeatType) == BeatType)
        {
            if (!_shootOnCooldown)
            {
                Shoot();
                _shootOnCooldown = true;
            }
        }
        else
        {
            _shootOnCooldown = false;
        }
    }

    private void Shoot()
    {
        Instantiate(Bullet, transform.position, _bulletRotation).GetComponent<Rigidbody2D>().AddRelativeForce(BulletForce, ForceMode2D.Force);
    }

    public void Spawn()
    {
        if (!_spawned)
        {
            _spawned = true;
            StartCoroutine(AnimateBossIn());
        }
        else
        {
            StartCoroutine(StartDelayedBossFight());
        }
    }

    private void StartBossFight()
    {
        _bossFightStarted = true;
        StartCoroutine(FlashScreen(true));
    }

    private void KillBoss()
    {
        BossRigidBody.simulated = false;
        StartCoroutine(PlayExplosions());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "base_player" && !Player.PlayerLogic.getIsDodging())
        {
            Player.Damage(-1);
        }
        else if (collision.gameObject.tag == "sample_playerProjectile" || collision.gameObject.tag == "sample_explosion")
        {
            if (_bossFightStarted)
            {
                Health -= 1;

                if (Health <= 0)
                {
                    Health = 0;
                    KillBoss();
                }

                HealthText.text = Health.ToString();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "sample_playerProjectile" || collision.gameObject.tag == "sample_explosion")
        {
            if (_bossFightStarted)
            {
                Health -= 1;

                if (Health <= 0)
                {
                    Health = 0;
                    KillBoss();
                }

                HealthText.text = Health.ToString();
            }
        }
    }

    private IEnumerator AnimateBossIn()
    {
        float timeElapsed = 0;

        while (timeElapsed < spawnDelay)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed -= spawnDelay;

        transform.localScale = StartScale;
        BossRenderer.material.color = StartColor;
        BossRenderer.enabled = true;

        while (timeElapsed < AnimationInTime)
        {
            transform.localScale = Vector2.Lerp(StartScale, TargetScale, timeElapsed / AnimationInTime);
            BossRenderer.material.color = Color.Lerp(StartColor, FullWarningColor, timeElapsed / AnimationInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = TargetScale;
        BossRenderer.material.color = HarmfulColor;

        BossRigidBody.simulated = true;
        Flip_CameraShake.Shake(HarmfulTime, 0.05f);

        StartCoroutine(AnimateToNeutral());
    }

    private IEnumerator AnimateToNeutral()
    {
        float timeElapsed = 0;

        while (timeElapsed < AnimateOutTime)
        {
            BossRenderer.material.color = Color.Lerp(HarmfulColor, EndColor, timeElapsed / AnimateOutTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        BossRenderer.material.color = EndColor;
    }

    private IEnumerator StartDelayedBossFight()
    {
        float timeElapsed = 0;

        while (timeElapsed < fightStartDelay)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        StartBossFight();
    }

    private IEnumerator FlashScreen(bool start)
    {
        ScreenFadeImage.color = new Color(1, 1, 1, 0.25f);

        if (start)
        {
            HealthIndicator.SetActive(true);
        }
        else
        {
            BossRenderer.enabled = false;
        }

        float timeElapsed = 0;

        while (timeElapsed < ScreenFlashDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        ScreenFadeImage.color = new Color(0, 0, 0, 0);

        if (!start)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayExplosions()
    {
        for (int i = 0; i < 8; i++)
        {
            Instantiate(Explosion,
                new Vector2(Random.Range(ExplosionMinPosition.x, ExplosionMaxPosition.x), Random.Range(ExplosionMinPosition.y, ExplosionMaxPosition.y)),
                Quaternion.identity);

            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(FlashScreen(false));
    }
}
