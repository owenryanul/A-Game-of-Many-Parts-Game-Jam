using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface for handling the listeners for when the Screen Fades in and Fades out.
public interface FadeEffectsListener
{
    //Called when the screen has fully faded to black, and this listener is subscribed(ScreenFadeEventsRelay.addListener()).
    public void OnFadeOutDone();

    //Called when the black screen has fulled faded away, and this listener is subscribed(ScreenFadeEventsRelay.addListener()).
    public void OnFadeInDone();

}
