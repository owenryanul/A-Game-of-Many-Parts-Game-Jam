using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_PraticeTargetLogic : MonoBehaviour
{
    [Header("Listener")]
    public List<Sample_LeverListener> listeners;

    [Header("Visuals")]
    public GameObject deathParticleEmitter;

    private bool dying;

    // Start is called before the first frame update
    void Start()
    {
        dying = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "sample_playerProjectile")
        {
            die();
        }
    }

    private void die()
    {
        if (!dying)
        {
            dying = true;
            foreach (Sample_LeverListener aListener in listeners)
            {
                aListener.onLeverStateChanged(true);
            }
            Instantiate(deathParticleEmitter, this.gameObject.transform.position, this.gameObject.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
