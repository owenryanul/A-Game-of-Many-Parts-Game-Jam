using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_SplashTextLogic : MonoBehaviour
{
    public float timeOnScreen;
    private bool wasShown;
    private float currentTimeLeftOnScreen;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Animator>().SetTrigger("FadeIn");
        currentTimeLeftOnScreen = timeOnScreen;
        wasShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!wasShown)
        {
            currentTimeLeftOnScreen -= Time.deltaTime;
            if (currentTimeLeftOnScreen <= 0)
            {
                wasShown = true;
                this.gameObject.GetComponent<Animator>().SetTrigger("FadeOut");
            }
        }
    }


}
