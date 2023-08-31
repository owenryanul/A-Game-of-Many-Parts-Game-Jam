using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Interface containing the listeners for when the player character dies.
public interface OnDeathListener
{
    //Called when the player character dies while this listener is subscribed to the player(PlayerLogic.addOnDeathListener())
    public void OnPlayerDies(PlayerLogic playerLogic);
}
