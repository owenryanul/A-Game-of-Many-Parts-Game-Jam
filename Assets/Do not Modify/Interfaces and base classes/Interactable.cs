using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface that marks a script's gameobject as being an interactable/
public interface Interactable
{
    //Called when the player hit's the interact key while this is the current interactable.
    public void onInteracted();

    //Called when this gameobject becomes the current interactable.
    public void onBecomeCurrentInteractable();

    //Called when this gameobject ceases to be the current interactable.
    public void onNoLongerCurrentInteractable();
}
