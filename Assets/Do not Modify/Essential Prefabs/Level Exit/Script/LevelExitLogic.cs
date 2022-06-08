using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitLogic : MonoBehaviour, FadeEffectsListener
{
    public string sceneToLoad;
    private bool calledSceneChanged;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<ScreenFadeEventsRelay>().addListener(this);
        calledSceneChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "base_player")
        {
            calledSceneChanged = true;
            GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<Animator>().SetTrigger("FadeOut");
        }
    }

    public void OnFadeOutDone()
    {
        if (calledSceneChanged)
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }

    public void OnFadeInDone()
    {
        //No Effect
    }
}
