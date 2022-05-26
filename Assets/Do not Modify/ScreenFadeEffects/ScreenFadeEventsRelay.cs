using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFadeEventsRelay : MonoBehaviour
{
    private List<FadeEffectsListener> listeners;

    private void OnEnable()
    {
        listeners = new List<FadeEffectsListener>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFadeOutDone()
    {
        foreach(FadeEffectsListener aListener in listeners)
        {
            aListener.OnFadeOutDone();
        }
    }

    public void OnFadeInDone()
    {
        foreach (FadeEffectsListener aListener in listeners)
        {
            aListener.OnFadeInDone();
        }
    }

    public void addListener(FadeEffectsListener listenerToAdd)
    {
        this.listeners.Add(listenerToAdd);
    }

    public void removeListener(FadeEffectsListener listenerToRemove)
    {
        this.listeners.Add(listenerToRemove);
    }
}
