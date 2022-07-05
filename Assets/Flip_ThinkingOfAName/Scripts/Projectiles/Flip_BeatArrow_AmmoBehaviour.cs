using Flip_SynchronizerData;
using UnityEngine;

public class Flip_BeatArrow_AmmoBehaviour : MonoBehaviour, AmmoBehaviour
{
    public GameObject ArrowPrefab;
    public AudioSource AudioSource;
    public float CoolDown;
    public BeatType BeatType;
    private Flip_BeatObserver _arrowBeatObserver;

    private float _timeElapsed;
    private bool _onCooldown;

    private void Start()
    {
        _arrowBeatObserver = GameObject.Find("ArrowBeatObserver").GetComponent<Flip_BeatObserver>();
    }

    private void Update()
    {
        if (!_onCooldown)
        {
            return;
        }

        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= CoolDown)
        {
            _onCooldown = false;
            _timeElapsed = 0;
        }
    }

    public void OnEquip(PlayerLogic playerLogic)
    {
        // No Effect.
    }

    public void OnFirePressd(PlayerLogic playerLogic)
    {
        if (_onCooldown)
        {
            return;
        }

        if ((_arrowBeatObserver.beatMask & BeatType) == BeatType)
        {
            GameObject arrow = Instantiate(ArrowPrefab, playerLogic.gameObject.transform.position, playerLogic.gameObject.transform.rotation);
            var arrowProjectile = arrow.GetComponent<Flip_BeatArrowProjectileLogic>();
            arrowProjectile.fireInDirection(playerLogic.getAimDirection());
            arrowProjectile.ArrowBeatExplosionObserver = _arrowBeatObserver;
            playerLogic.modifyAmmoAmount(playerLogic.getAmmoRelativeToCurrent(0), -1);
        }
        else
        {
            AudioSource.Play();
        }

        _onCooldown = true;
    }

    public void OnFireCancelled(PlayerLogic playerLogic)
    {
        // No Effect.
    }

    public void OnFireReleased(PlayerLogic playerLogic)
    {
        // No Effect.
    }

    public void OnUnequip(PlayerLogic playerLogic)
    {
        // No Effect.
    }
}
