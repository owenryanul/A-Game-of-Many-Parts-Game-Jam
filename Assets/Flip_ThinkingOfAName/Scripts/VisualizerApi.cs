using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VisualizerApi : MonoBehaviour
{
	private AudioSource _audioSource;
	public static float[] _samples = new float[512];
	public static float[] _freqBand = new float[8];
	public static  float[] _bandBuffer = new float[8];
	public static float[] _bufferDecrease = new float[8];

	private float[] _freqBandHighest = new float[8];
	public static float[] audioBand = new float[8];
	public static float[] audioBandBuffer = new float[8];
	public static float amplitude, amplitudeBuffer;
	private float _amplitudeHighest;

	public int audioProfile;

	// Use this for initialization
	void Start ()
	{
		_audioSource = GetComponent<AudioSource>();
		audioProfile = 1;
		InitAudioProfile();
	}

	// Update is called once per frame
	void Update () {
		GetSpectrum();
		MakeFrequencyBands();
		BandDeccay();
		CreateAudioBands();
		GetAmplitude();
	}

	private void InitAudioProfile()
	{
		for (int i = 0; i < 8; i++)
		{
			_freqBandHighest[i] = audioProfile;
		}
	}

	private void GetSpectrum()
	{
		_audioSource.GetSpectrumData(_samples,0,FFTWindow.Blackman);
	}

	private void MakeFrequencyBands()
	{
		///43 Hx per samples if 512 samples

		/**

		frequency bands is working out of power of 2, per bands

		*/
		int count = 0;

		for (int i = 0; i < 8; i++)
		{
			float average = 0;
			int sampleCount = (int) Mathf.Pow(2, i + 1);
			if (i == 7)
			{
				sampleCount += 2; //otherwwise we have only 510 sampes
			}

			for (int j = 0; j < sampleCount; j++)
			{
				average += _samples[count] * (count + 1);
				count++;
			}
			average /= count;

			_freqBand[i] = average * 10f;

			if (_audioSource.volume > 0.1f)
			{
				_freqBand[i] /= _audioSource.volume;
			}
			else
			{

				_freqBand[i] = 0;
			}

		}
	}

	private void BandDeccay()
	{
		for (int g = 0; g < 8; g++)
		{
			if (_freqBand[g] > _bandBuffer[g])
			{
				_bandBuffer[g] = _freqBand[g];
				_bufferDecrease[g] = 0.005f;
			}

			if (_freqBand[g] <= _bandBuffer[g])
			{
				_bandBuffer[g] -= _bufferDecrease[g];
				_bufferDecrease[g] *= 1.1f;
			}

		}
	}

	private void CreateAudioBands()
	{
		for (int i = 0; i < 8; i++)
		{
			if (_freqBand[i]>_freqBandHighest[i])
			{
				_freqBandHighest[i] = _freqBand[i];
			}
			audioBand[i] = _freqBand[i] / _freqBandHighest[i];
			audioBandBuffer[i] = _bandBuffer[i] / _freqBandHighest[i];
		}
	}

	private void GetAmplitude()
	{
		float currentAmplidue= 0;
		float currentAmplidueBuffer = 0;
		for (int i = 0; i < 8; i++)
		{
			currentAmplidue += audioBand[i];
			currentAmplidueBuffer += audioBandBuffer[i];
		}

		if (currentAmplidue > _amplitudeHighest)
		{
			_amplitudeHighest = currentAmplidue;
		}



		amplitude = currentAmplidue / _amplitudeHighest;
		amplitudeBuffer = currentAmplidueBuffer / _amplitudeHighest;

	}
}
