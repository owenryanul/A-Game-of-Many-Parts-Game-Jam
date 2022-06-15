ScreenFadeEffects Readme:

	The ScreenFadeEffects prefab is a gameobject that provides fade to black and fade in visual effects to the camera, to assit in scene transitions.

	Only a single instance of the ScreenFadeEffects should by present in a scene and it should be attached to a ui overlay covering the main camera.
	
	FadeEffectsListener:
	FadeEffectsListener is an interface used for listening to FadeIn and FadeOut events triggered by SceenFadeEffects.

	OnFadeInDone() is called when ScreenFadeEffects finishes its FadeInAnimation

	OnFadeOutDone() is called when ScreenFadeEffects finishes its FadeOutAnimation

	Use ScreenFadeEventsRelay.addListener() to make a script implementing FadeEffecstsListener listen to these events.