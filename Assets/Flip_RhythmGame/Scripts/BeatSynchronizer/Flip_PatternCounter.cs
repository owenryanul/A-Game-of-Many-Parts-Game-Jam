﻿// License
// The MIT License (MIT)
//
// Copyright (c) 2014 Christian Floisand
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections;
using Flip_SynchronizerData;

/// <summary>
/// This class is responsible for counting and notifying its observers when a beat in the specified pattern sequence occurs.
/// The accuracy of the counter is handled by loopTime, which controls how often it checks whether a beat has happened.
/// Higher settings for loopTime decreases load on the CPU, but will result in less accurate beat synchronization.
/// </summary>
public class Flip_PatternCounter : MonoBehaviour {

	public BeatValue[] beatValues;
	[Tooltip("This value acts as a multiplier for all the beat value specified in the pattern, allowing for the " +
	         "sequence to extend beyond a single measure. Range: 1-10")]
	public int beatScalar = 1;
	[Tooltip("Controls the frequency that the counter checks for beats. In milliseconds.")]
	public float loopTime = 30f;
	public AudioSource audioSource;
	public GameObject[] observers;

	private float nextBeatSample;
	private float[] samplePeriods;
	private int sequenceIndex;
	private float currentSample;


	void Awake ()
	{
		float audioBpm = audioSource.GetComponent<Flip_BeatSynchronizer>().Bpm;
		samplePeriods = new float[beatValues.Length];

		// Calculate number of samples between each beat in the sequence.
		for (int i = 0; i < beatValues.Length; ++i) {
			samplePeriods[i] = (60f / (audioBpm * BeatDecimalValues.values[(int)beatValues[i]])) * audioSource.clip.frequency;
			samplePeriods[i] *= beatScalar;
		}

		nextBeatSample = 0f;
		sequenceIndex = 0;
	}

	/// <summary>
	/// Initializes and starts the coroutine that checks for beat occurrences in the pattern. The nextBeatSample field is initialized to
	/// exactly match up with the sample that corresponds to the time the audioSource clip started playing (via PlayScheduled).
	/// </summary>
	/// <param name="syncTime">Equal to the audio system's dsp time plus the specified delay time.</param>
	void StartPatternCheck (double syncTime)
	{
		nextBeatSample = (float)syncTime * audioSource.clip.frequency;
		StartCoroutine(PatternCheck());
	}

	/// <summary>
	/// Subscribe the PatternCheck() coroutine to the beat synchronizer's event.
	/// </summary>
	void OnEnable ()
	{
		Flip_BeatSynchronizer.OnAudioStart += StartPatternCheck;
	}

	/// <summary>
	/// Unsubscribe the PatternCheck() coroutine from the beat synchronizer's event.
	/// </summary>
	/// <remarks>
	/// This should NOT (and does not) call StopCoroutine. It simply removes the function that was added to the
	/// event delegate in OnEnable().
	/// </remarks>
	void OnDisable ()
	{
		Flip_BeatSynchronizer.OnAudioStart -= StartPatternCheck;
	}

	/// <summary>
	/// This method checks if a beat has occurred in the pattern's sequence by comparing the current sample position of the audio
	/// versus the next expected sample value. The frequency of the checks is controlled by the loopTime field.
	/// </summary>
	/// <remarks>
	/// The WaitForSeconds() yield statement places the execution of the coroutine right after the Update() call, so by
	/// setting the loopTime to 0, this method will update as frequently as Update(). If even greater accuracy is
	/// required, WaitForSeconds() can be replaced by WaitForFixedUpdate(), which will place this coroutine's execution
	/// right after FixedUpdate().
	/// </remarks>
	IEnumerator PatternCheck ()
	{
		while (audioSource.isPlaying) {
			currentSample = (float)AudioSettings.dspTime * audioSource.clip.frequency;

			if (currentSample >= nextBeatSample) {
				foreach (GameObject obj in observers) {
					// Since this is a specific pattern of beats, we don't need to track different beat types.
					// Instead, client can index a custom beat counter to track which beat in the sequence has fired.
					obj.GetComponent<Flip_BeatObserver>().BeatNotify();
				}
				nextBeatSample += samplePeriods[sequenceIndex];
				sequenceIndex = (++sequenceIndex == beatValues.Length ? 0 : sequenceIndex);
			}

			yield return new WaitForSeconds(loopTime / 1000f);
		}
	}

}
