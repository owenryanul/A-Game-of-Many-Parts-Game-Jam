using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class QuitAndRetryLogic : MonoBehaviour, FadeEffectsListener
{
    [Header("Quit")]
    public float delayOnQuitButton = 3.0f;
    private float timeTilQuit;
    private bool quitButtonHeld;
    private bool quiting;

    [Header("Retry")]
    private float timeTilRetry;
    private bool retryButtonHeld;
    private bool retrying;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<ScreenFadeEventsRelay>().addListener(this);

        timeTilQuit = delayOnQuitButton;
        quitButtonHeld = false;
        quiting = false;
        GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().enabled = false;

        timeTilRetry = delayOnQuitButton;
        retryButtonHeld = false;
        retrying = false;

    }

    // Update is called once per frame
    void Update()
    {
        quitLogic();
        retryLogic();
    }

    //===========================[Quit and Retry]========================================

    public void OnQuitButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!retryButtonHeld)
            {
                quitButtonHeld = true;
                GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().enabled = true;
                GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().text = "Quitting...";
            }
        }
        else if (context.canceled)
        {
            try
            {
                quitButtonHeld = false;
                timeTilQuit = this.delayOnQuitButton;
                if (!retryButtonHeld)
                {
                    GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
            catch (NullReferenceException e)
            {
                //quit text will throw an exception as the scene doesn't quit before the key is released but this is harmless
            }
        }
    }

    private void quitLogic()
    {
        if (quitButtonHeld)
        {
            timeTilQuit -= Time.deltaTime;
            Color aColor = Color.white;
            aColor.a = 1.0f - (0.8f * (timeTilQuit / delayOnQuitButton));
            GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().color = aColor;

            if (timeTilQuit <= 0)
            {
                quiting = true;
                GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<Animator>().SetTrigger("FadeOut");
            }
        }
    }

    public void OnRetryButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!quitButtonHeld)
            {
                retryButtonHeld = true;
                GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().enabled = true;
                GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().text = "Retrying...";
            }
        }
        else if (context.canceled)
        {
            try
            {
                retryButtonHeld = false;
                timeTilRetry = this.delayOnQuitButton;
                if (!quitButtonHeld)
                {
                    GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
            catch (NullReferenceException e)
            {
                //quit text will throw an exception as the scene doesn't quit before the key is released but this is harmless
            }
        }
    }

    private void retryLogic()
    {
        if (retryButtonHeld)
        {
            timeTilRetry -= Time.deltaTime;
            Color aColor = Color.white;
            aColor.a = 1.0f - (0.8f * (timeTilRetry / delayOnQuitButton));
            GameObject.FindGameObjectWithTag("base_quitText").GetComponent<TextMeshProUGUI>().color = aColor;

            if (timeTilRetry <= 0)
            {
                retrying = true;
                GameObject.FindGameObjectWithTag("base_screenFadeEffects").GetComponent<Animator>().SetTrigger("FadeOut");
            }
        }
    }

    public void OnFadeOutDone()
    {
        if(quiting)
        {
            if (SceneManager.GetActiveScene().name == "base_Lobby")
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene("base_Lobby", LoadSceneMode.Single);
            }
        }
        else if(retrying)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    public void OnFadeInDone()
    {
        //No Effect
    }

    //===========================[End of Quit and Retry]========================================
}
