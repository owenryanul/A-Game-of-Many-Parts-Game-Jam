Level Exit Readme:

	The level exit triggers scene transitions by working with an instance of the ScreenFadeEffects prefab.
	When the player collides with the level exit it will trigger the ScreenFadeEffect object's FadeOut Animation.
	Once the FadeOut Animation is complete it triggers an animation event, which causes the ScreenEffect object to call all OnFadeOutDone listeners.
	Level exit is one of those listeners and will, on receiving OnFadeOutDone(), assuming it was what triggered the FadeOut Animation,
	switch to a scene with the name matching the one entered in the inspector.

	All of this just ensures that any scene transition is masked by a fade to black effect.