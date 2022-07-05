using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_ProjectileParentNotifier : MonoBehaviour
{
    private Oryan_ProjectileParent parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        parent.childProjectileDestroyed();
    }

    public void setParent(Oryan_ProjectileParent parentIn)
    {
        parent = parentIn;
    }

}
