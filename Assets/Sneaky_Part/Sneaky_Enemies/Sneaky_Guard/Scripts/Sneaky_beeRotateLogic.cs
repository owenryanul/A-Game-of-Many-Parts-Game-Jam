using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sneaky_beeRotateLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public float rotationSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
