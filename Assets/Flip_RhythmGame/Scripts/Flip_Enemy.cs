using System.Collections;
using Flip_SynchronizerData;
using UnityEngine;

public class Flip_Enemy : MonoBehaviour, Flip_ISpawnable
{
    public Flip_BeatObserver BeatObserver;
    public BeatType BeatType;

    public Renderer EnemyRenderer;
    public Rigidbody2D EnemyRigidBody;

    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public AnimationCurve XPositionAnimationCurve;
    public AnimationCurve YPositionAnimationCurve;

    public GameObject Bullet;
    public Vector2 BulletForce;
    public Vector3 BulletRotation;

    private bool _canShoot;
    private bool _shootOnCooldown;

    private const float spawnDelay = 2;

    public void Start()
    {
        transform.position = StartPosition;
        EnemyRenderer.enabled = false;
        EnemyRigidBody.simulated = false;
    }

    public void Update()
    {
        if (!_canShoot)
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
        Instantiate(Bullet, transform.position, Quaternion.Euler(BulletRotation)).GetComponent<Rigidbody2D>().AddForce(BulletForce, ForceMode2D.Force);
    }

    public void Spawn()
    {
        EnemyRigidBody.simulated = true;
        StartCoroutine(SpawnEnemyAnimation(spawnDelay));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "sample_playerProjectile" || collision.gameObject.tag == "sample_explosion")
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "sample_playerProjectile" || collision.gameObject.tag == "sample_explosion")
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator SpawnEnemyAnimation(float delay)
    {
        float timeElapsed = 0;

        while (timeElapsed < delay)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed -= delay;

        EnemyRenderer.enabled = true;

        while (timeElapsed < (XPositionAnimationCurve[XPositionAnimationCurve.length - 1].time))
        {
            var animationCurveValues = new Vector2(XPositionAnimationCurve.Evaluate(timeElapsed), YPositionAnimationCurve.Evaluate(timeElapsed));
            transform.position = new Vector2(Mathf.Lerp(StartPosition.x, EndPosition.x, animationCurveValues.x), Mathf.Lerp(StartPosition.y, EndPosition.y, animationCurveValues.y));

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = EndPosition;
        _canShoot = true;
    }
}
