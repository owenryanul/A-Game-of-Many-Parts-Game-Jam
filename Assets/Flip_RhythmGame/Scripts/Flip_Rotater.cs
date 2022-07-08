using UnityEngine;
using Random = UnityEngine.Random;

public class Flip_Rotater : MonoBehaviour
{
    public float RotationSpeedMin;
    public float RotationSpeedMax;
    private float _speed;

    public void Start()
    {
        _speed = Random.Range(RotationSpeedMin, RotationSpeedMax);
    }

    public void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, _speed * Time.deltaTime));
    }
}
