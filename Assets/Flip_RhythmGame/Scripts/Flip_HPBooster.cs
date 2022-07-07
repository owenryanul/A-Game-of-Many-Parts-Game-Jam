using UnityEngine;

public class Flip_HPBooster : MonoBehaviour, Interactable
{
    public PlayerLogic PlayerLogic;
    private bool _isOn = true;

    public void onInteracted()
    {
        if (_isOn)
        {
            PlayerLogic.addHp(3);
            _isOn = false;
        }
    }

    public void onBecomeCurrentInteractable()
    {
        // No effect.
    }

    public void onNoLongerCurrentInteractable()
    {
        // No effect.
    }
}
