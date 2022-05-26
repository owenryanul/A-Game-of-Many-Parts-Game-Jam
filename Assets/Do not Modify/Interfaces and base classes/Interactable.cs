using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public void onInteracted();

    public void onBecomeCurrentInteractable();

    public void onNoLongerCurrentInteractable();
}
