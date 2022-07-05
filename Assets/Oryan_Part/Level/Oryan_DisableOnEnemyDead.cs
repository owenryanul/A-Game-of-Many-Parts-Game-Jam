using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_DisableOnEnemyDead : MonoBehaviour
{
    public List<GameObject> watchers;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool allClear = true;
        foreach (GameObject anEnemy in watchers)
        {
            if(anEnemy != null)
            {
                allClear = false;
                break;
            }
            
        }

        if (allClear)
        {
            Destroy(this.gameObject);
        }
    }
}
