using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_GlobalDataHolder : MonoBehaviour
{
    public int lightSourcesLightingThePlayer;

    private void OnEnable()
    {
        lightSourcesLightingThePlayer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isThePlayerLit()
    {
        Debug.Log("Player is lit by" + lightSourcesLightingThePlayer);
        if(lightSourcesLightingThePlayer > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
