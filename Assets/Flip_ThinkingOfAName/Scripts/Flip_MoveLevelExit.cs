using UnityEngine;

public class Flip_MoveLevelExit : MonoBehaviour
{
    public Rigidbody2D LevelExit;
    public Vector2 Force;

    private bool _doOnce;

    public void Move()
    {
        if (_doOnce)
        {
            return;
        }

        _doOnce = true;
        LevelExit.AddForce(Force, ForceMode2D.Force);
    }
}
