using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_SpawnerLogic : Sample_LeverListener
{
    public GameObject objectToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void onLeverStateChanged(bool isNowOn)
    {
        Instantiate(objectToSpawn, this.gameObject.transform.position, this.gameObject.transform.rotation);
    }
}
