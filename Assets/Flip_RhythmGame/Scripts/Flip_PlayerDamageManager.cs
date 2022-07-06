using UnityEngine;

public class Flip_PlayerDamageManager : MonoBehaviour
{
    public PlayerLogic PlayerLogic;
    public float InvulnerabilityTime;

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
    }
}
