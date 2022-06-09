A breakdown of the various hooks featured in PlayerLogic.cs


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
	-carried ammo
	-default ammo
	-getAmmoRelativeToCurrent
	-current ammo index(make private?)
	-change ammo type.
	-ammo type class
	-Ammo behaviour class
		-on equip
		-on unequip
		-on fire pressed
		-on fire released
		-on (fire) canceled

	AmmoType Class:
		This 

Fire:
	-calls OnFireFor the current ammo behaviour

Activate:
	-Activateable Listener Class
		-OnActivatePressed
		-OnActivateReleased
	-Activateable get/set/is
	
Interact:
	-Interactable Class
	-get/set current Interactable

Dodge Roll:
	-Dodge Roll Velocity
	-is dodge rolling

Hp:
	-add/set/get hp
	-death
	-on death listeners
	-respawn

Status:
	-Status System
	-Add status
	-get status
	-remove status

Paused:
	-paused

Audio:
	-audio files nothing to worry about

Misc:
	-


