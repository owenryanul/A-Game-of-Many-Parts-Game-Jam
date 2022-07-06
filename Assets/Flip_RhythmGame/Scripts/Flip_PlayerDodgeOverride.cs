using UnityEngine;
using UnityEngine.InputSystem;

public class Flip_PlayerDodgeOverride : MonoBehaviour
{
    public PlayerLogic PlayerLogic;
    public float DodgeCoolDown;
    private float _coolDownTimer;

    private bool _canDodge = true;
    private Vector2 _lastMovementDirection;

    void Update()
    {
        UpdateDodgeCooldown();
        CheckDodgeCancel();
    }

    private void UpdateDodgeCooldown()
    {
        if (_canDodge)
        {
            return;
        }

        _coolDownTimer += Time.deltaTime;

        if (_coolDownTimer >= DodgeCoolDown)
        {
            _canDodge = true;
            _coolDownTimer = 0;
        }
    }

    private void CheckDodgeCancel()
    {
        if (PlayerLogic.getIsDodging())
        {

        }

        bool switchedDirection = false;
        Vector2 movementDirection = PlayerLogic.getPlayerMovementDirection();

        if (movementDirection.x > 0 && _lastMovementDirection.x < 0)
        {
            switchedDirection = true;
        }

        if (movementDirection.x < 0 && _lastMovementDirection.x > 0)
        {
            switchedDirection = true;
        }

        if (movementDirection.y > 0 && _lastMovementDirection.y < 0)
        {
            switchedDirection = true;
        }

        if (movementDirection.y < 0 && _lastMovementDirection.y > 0)
        {
            switchedDirection = true;
        }

        if (PlayerLogic.getIsDodging() && switchedDirection)
        {
            PlayerLogic.gameObject.GetComponent<Animator>().Play("Player Idle Animation");
            PlayerLogic.OnDodgeAnimationDone();
        }

        if (movementDirection.x != 0)
        {
            _lastMovementDirection = new Vector2(movementDirection.x, _lastMovementDirection.y);
        }

        if (movementDirection.y != 0)
        {
            _lastMovementDirection = new Vector2(_lastMovementDirection.x, movementDirection.y);
        }
    }

    public void OnDodgePressed(InputAction.CallbackContext context)
    {
        if (_canDodge && context.phase == InputActionPhase.Performed)
        {
            _canDodge = false;
            PlayerLogic.OnDodgePressed(context);
        }
    }
}
