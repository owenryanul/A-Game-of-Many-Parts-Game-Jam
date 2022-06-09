A breakdown of the various hooks featured in PlayerLogic.cs

controller deadzones

Camera:

	Player Camera: 
		The main camera that is being used by the player. Set this to whatever camera you're using as your main.

	Player Camera follows player.
		This toggles whether or not Player Camera will move to the players position at all times.




Movement:

	getMovementDirection():
		Returns the current vector that the player's input is trying to move the player character in.

	topSpeed:
		The maximum speed the player character is able to run at, in units per second.

	acceleration:
		The accerleration over time when the player character is running, units per second per second.

	deceleration:
		The deceleration over time when the player movementDirection is zero, i.e. the player is not inputting any direction.

	External Velocity:
		A vector that is added to the player's velocity after their normal movement has been calculated. Use this vector if you want to apply movement to the player from
		an external source, e.g. a conveyor belt of fan. 
		Can either be set or added too.
		If overridePlayerMovementVelocity is set to true, then External Velocity will replace player movement instead of being added to it.

	overridePlayerMovementVelocity:
		Determines whether or not External Velocity will replace player movement or be added to it. True for replace, false for added to. Defaults to false.

	A Note on player movement:
		Internally, player movement is done by setting the velocity of their rigidbody rather than applying forces, bear this in mind if you wish to alter the properties of the 
		player character's rigidbody.




Aiming:

	getAimDirection():
		Returns the vector between the player character and where the player is currently aiming with the mouse or right stick.
		Note: the vector is NOT normalized.
		Note 2: If you plan to use the exact location the player is aiming at, remember that on controller they will only be able to aim a certain distance 
		away from the player character compared to with a mouse.

	maxReticleDistanceFromPlayer:
		The maximium distance from the player that the aiming reticule can extend out from the player when trying to reach the exact point the player is aiming. If the player
		aiming beyond this range, the reticle will stop at this range.




Ammo:


	The Ammo System:
		The player has a collection of different ammo types that developers can populate. The player can switch between and fire them. It works by giving the player a list of
		"Ammo" objects and allowing the player to cycle through the list, changing the currently equiped ammo. When the player switches to using a specfic ammo type, it will
		create an instance of that "Ammo"'s "AmmoBehaviour", a game object with a script attached that implements the AmmoBehaviour Interface, allowing said script to listen for
		specific events from PlayerLogic and take action based on them. Once the player switches off the current ammo type, PlayerScript will destroy the currently active
		AmmoBehaviour object and create an instance of the new current "Ammo"'s "AmmoBehaviour" object.



	Adding a new Ammo Type:
		1. Create a gameobject, preferable a blank one.
		2. Add a script to the object that implements the AmmoBehaviour Interface.
		3. Save the gameObject as a prefab.
		4. Create another script that calls PlayerLogic.addAmmo(yourNewAmmoType). Passing it an instance of Ammo with a unique name and your AmmoBehaviour prefab.
		5. Add any logic to support what you want the new ammo to do, to your AmmoBehaviour script, e.g. spawn projectiles.



	Ammo Class:
		This class is used for storing different ammunition types for the player. Each Ammo object has a name used for indentifying it. An icon used in the ammo indicator. A quanity
		showing how much of it is left. And finally a GameObject containing the ammo's AmmoBehaviour(see below).



	AmmoBehaviour Interface:
		This interface should be implemented by the logic attached to any AmmoBehaviour gameObject. It provides the listeners that PlayerLogic will call on to interact with the
		currently equiped ammo type. The Listeners are:
			-OnEquiped(): 
				Called when the player switches to the ammo type.
			-OnUnEquiped():
				Called when the player switches off of the ammo type.
			-OnFirePressed():
				Called when the player fires down on the fire button.
			-OnFireReleased():
				Called when the player releases the fire button. Not called if OnFireCancelled() has been called since OnFirePressed() was last called.
			-OnFireCancelled():
				Called when the player changes ammo type or dodge rolls after pressing fire but before releasing fire. Use if either would interupt some behaviour with your ammo.



	carriedAmmo:
		List of the Ammo storing all ammo types currently carried by the player.

	defaultAmmo:
		Ammo object used as the default if carriedAmmo is empty.
		By default this field is set to NoAmmo, an ammo with no effects whatsoever.

	getAmmoFromName(string name):
		Returns the ammo in carriedAmmo with the matching name.
		Returns null if there is no ammo with the matching name in carriedAmmo.

	getAmmoRelativeToCurrent(int offset)
		Method primarily used by the Ammo Indicator. Developers may prefer to use getAmmoFromName
		Returns the ammo in carriedAmmo that is offset places in the list from the currently equiped ammo. 
		e.g 0 returns the current ammo. 2 returns the ammo 2 places later in the list. -1 returns the ammo just before the current ammo, etc.
		Will loop through the list if the offset would go out of the bounds of the list.
		e.g. in a list of size 3 with position 1 being current, an offset of 2 would return the ammo in position 0.

	addAmmo(Ammo ammoToAdd):
		Adds the provided ammo to the player's list of carried ammo types. If the ammo already exists in the list then add ammoToAdd.quantity to the existing ammo's quantity.

	modifyAmmoAmount(Ammo ammoToModify, int amount)
		Finds the passed ammo type in the player's list of carried ammo and modifies it by the pass amount. 
		If the ammo's new quantity drops to 0 then it is removed it from the list.
		Returns false if it cannot find the matching ammo type, otherwise returns true.
		



Activate:

	Activate System:
		The player can hit the activate button to perform various special actions. E.g. triggering bomb arrows.
		This works by giving the player a list of scripts that implement the OnActivateListener Interface which will then receive events when the the activate button is pressed.


	-Activateable Listener Class
		-OnActivatePressed
		-OnActivateReleased
	-Activateable get/set/is

	OnActivateListener Interface:
		Interface used by scripts that want to listen to player events related to the activate button. 
		Has the following Listeners:
			-OnActivatePressed():
				Called when the player presses the activate button.
			-OnActivateReleased():
				Called when the player releases the activate button.

	addOnActivateListener(OnActivateListener listenerIn):
		Adds the passed script to the list of OnActivateListeners that receive OnActivate events.

	removeOnActivateListener(OnActivateListener listenerIn):
		Removes the passed script from the list of OnActivateListeners that receive OnActivate events.

(((((((((((((((((((Test this)))))))))))))))))))
	isAnOnActivateListener(OnActivateListener listenerIn):
		Returns true if the list of OnActivate Listeners contains listenerIn.




Interact:

	Interactable Interface:
		Interface used by scripts that want to listen to player events related to the interact button.
		Has the following Listeners:
			-onInteracted():
				Called when this script is the current interactable and the player hits the interact key.
			-onBecomeCurrentInteractable():
				Called when this script first becomes the current interactable.
			-onNoLongerCurrentInteractable():
				Called when this script stops being the current interactable.
	
	getCurrentInteractable():
		Returns the script currently set to receive any Interactable events. 

	setCurrentInteractable(Interactable inInteractable)
		Makes inInteractable the script that receives any Interacatable events.




Dodge Roll:

	
	-Dodge Roll Velocity
	-is dodge rolling

	dodgeRollVelocity:
		The velocity at which the player will move when dodge rolling.

	getIsDodging():
		Returns isDodging. Which is true when the player is in the middle of a dodge roll, and false otherwise. Use this to implement behaviour or special excepts when the player
		is dodge rolling. E.g. not taking damage from enemy if they collide when dodge rolling.

	setIsDodging(bool inDodging):
		Manually sets isDodging to inDodging. 
		Note: The player cannot start a dodge roll while isDodging is true, and finishing the dodgeRoll animation will set the isDodging to false.


Hp:

	setHp(int HPin, bool triggerHurtAnimation = false):
		Sets the player's current hp to HPin. If the new currentHP is 0 or less, the player character dies.
		If triggerHurtAnimation passed as true then this will trigger a damage animation and hurt sound effect.

	addHp(int HPin, bool triggerHurtAnimation = false):
		Modifies the player's current hp by HPin. If the new currentHP is 0 or less, the player character dies.
		If triggerHurtAnimation passed as true then this will trigger a damage animation and hurt sound effect.

	getHp():
		Returns the player's current hp.

	respawn():
		Sets the player's hp to 1 and marks them as not dead. Call when you want to revive the player character after they die.

	OnDeathListener Interface:
		Interface extended by scripts that want to be notified when the player dies. Has the following Listeners:
			-OnPlayerDies(): Called when the player character's death animation finishes.

	addOnDeathListener(OnDeathListener listenerIn):
		add the passed script to the list of OnDeathListeners that will receive OnDeath events.

	removeOnDeathListener(OnDeathListener listenerIn):
		removed the passed script from the list of OnDeathListeners that will receive OnDeath events.

	-add/set/get hp
	-death
	-on death listeners
	-respawn




Status:
	-Status System
	-Add status
	-get status
	-remove status

	Status System:
		The Status system for the player does nothing on it's own as it's just a way of storing id'ed strings attached to the player character. It is intended to all developers
		to apply persistant tags to the player if they wish.

	Status Class:
		The class used for the status system, consisting of a pair of strings. An id string used to identify the data, and the data string used to store the data.

	addStatus(string newID, string newData):
		Adds a new Status to the player's list of statuses with id = newId and data = newData.
		
	removeStatus(string idToRemove):
		Removes the Status from the player's list of statuses with id = idToRemove.

	getStatusData(string id):
		returns the data field of the status in the player's list of statuses with id = id.
		returns null if it can find no matching status in the player's list of statuses.

	modifyStatus(string id, string newData)::
		finds the status in the player's list of statuses with id = id and replace's it's data field with newData.
		Returns false if it cannot find a status with matching id in the player's list of statuses. Otherwise, returns true.




Paused:
	Paused:
		If true disables player inputs and most logic. If false, has no effect. Use to disable player inputs if needed.

Audio:
	Pain Sound:
		Audio clip used as the player's pain sound. Played when play their damage animation.

	Roll Sound:
		Audio clip used as the player's dodge roll sound. Played when they begin their dodge roll animation.
		

	Death Sound:
		Audio clip used as the player's death sound. Played when the player's death animation triggers.


Quit and Retry:
	
	DelayOnQuitButton:
		The time in seconds the player must hold down the quit or retry button before it triggers.
	


