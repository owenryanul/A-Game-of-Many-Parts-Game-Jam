using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface containing listeners for when the player uses the Activate key.
public interface OnActivateListener
{
    //Called when the player presses the Activate key while this listener is subscribed to the player(PlayerLogic.addOnActivateListener())
    public void OnActivatePressed();

    //Called when the player releases the Activate key while this listener is subscribed to the player(PlayerLogic.addOnActivateListener())
    public void OnActivateReleased();
}
