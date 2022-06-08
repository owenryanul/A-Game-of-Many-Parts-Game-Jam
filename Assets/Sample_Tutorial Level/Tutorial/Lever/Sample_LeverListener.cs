using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sample_LeverListener: MonoBehaviour
{
    //The reason LeverListener is a class and not an interface is so that it can be serialized and thereby lever listeners can be assigned in the inspector to public fields.

    public virtual void onLeverStateChanged(bool isNowOn)
    {
        throw new System.NotImplementedException("LeverListener should only be used as a baseclass.");
    }
}
