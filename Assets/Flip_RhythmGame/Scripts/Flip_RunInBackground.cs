using UnityEngine;

public class Flip_RunInBackground : MonoBehaviour
{
    public void Awake()
    {
        Application.runInBackground = true;
    }
}
