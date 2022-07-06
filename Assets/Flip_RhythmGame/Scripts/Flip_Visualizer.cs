using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flip_Visualizer : MonoBehaviour
{
    [Serializable]
    public struct Ring
    {
        public int NumberOfObjects;
        public float Radius;
    }

    [Serializable]
    public struct Line
    {
        public int NumberOfObjects;
        public float DistanceBetweenEachObject;
    }

    public GameObject Prefab;
    public Light PointLight;
    public Light Spotlight;

    [Header("Starting Parameters")]
    public Ring[] Rings;
    private int _trueNumberOfObjects;
    public Line LineVisualizer;
    public AudioSource AudioSource;

    [Tooltip("maxdB buffer")]
    public float InitialMaxdB;

    [Header("Visualizer Properties")]
    public float Scale;
    public bool CenterVisualizer;
    public bool Rotate;

    [Header("Bar Properties")]
    public bool AutoThreshold;
    public float HeightThreshold;
    public float ScaleSpeed;
    public float ColourScaleSpeed;
    public float ColourMultiplier;
    public float ScaleHeight;
    public bool Smoothing;

    [Header("Beat Properties")]
    public bool UseBeat;
    public Transform BeatObject;
    public int BeatSpectrumSample;
    public float BeatScaleHeight;
    public float BeatSizeThreshold;
    public float MinScale;
    public float BeatScaleSpeed;

    [Header("Point Light Properties")]
    public bool PointLightOn;
    public bool RandomizePointLight;
    [Tooltip("dB > maxdB - (maxdB / upperThreshold) to change color")]
    public float PointLightRandomizerUpperThreshold;
    [Tooltip("dB < maxdB - (maxdB / lowerThreshold) to allow another change color")]
    public float PointLightRandomizerLowerThreshold;
    public Color PointLightStaticColor;
    public float PointLightIntensity;

    [Header("Spotlight Properties")]
    public bool SpotlightOn;
    public bool RandomizeSpotlight;
    [Tooltip("dB > maxdB - (maxdB / upperThreshold) to change color")]
    public float SpotlightRandomizerUpperThreshold;
    [Tooltip("dB < maxdB - (maxdB / lowerThreshold) to allow another change color")]
    public float SpotlightRandomizerLowerThreshold;
    public Color SpotlightStaticColor;
    public float SpotlightIntensity;

    private List<Transform> _meters = new List<Transform>();
    private List<Material> _meterMats = new List<Material>();

    private float _rms; // sound level - RMS
    private float _dB; // sound level - dB
    private float _maxdB;
    private float _decreasingdB = 0;
    private float _decreaseRate = 0.005f;

    private const int qSamples = 1024; // array size
    private const float refRms = 0.1f; // RMS value for 0 dB

    private float[] _samples;
    private float[] _spectrum;
    private float[] _highestValues;

    private bool _changePointLightColor = false;
    private bool _changeSpotlightColor = false;

    public Material VisualizerMaterial;

    private float _highestMeter = 0;

    private void Start()
    {
        _maxdB = InitialMaxdB;

        _samples = new float[qSamples];
        _spectrum = new float[qSamples];
        _highestValues = new float[qSamples];

        GenerateCircleVisualizer();
        GenerateLineVisualizer();
    }

    private void FixedUpdate()
    {
        transform.localScale = new Vector3(Scale, 1, Scale);

        AnalyzeSound();
        VisualizeSound();

        if (Rotate == true)
            RotateVisualizer();
    }

    private void GenerateCircleVisualizer()
    {
        for (int j = 0; j < Rings.Length; j++)
        {
            float RandomStartingAngle = Random.Range(0f, 360f);
            _trueNumberOfObjects += Rings[j].NumberOfObjects;
            for (int i = 0; i < Rings[j].NumberOfObjects; i++)
            {
                float angle = (i * Mathf.PI * 2 / Rings[j].NumberOfObjects) + RandomStartingAngle;
                Vector3 center = transform.position;
                Vector3 pos;
                pos.x = center.x + Rings[j].Radius * Mathf.Sin(angle + transform.localRotation.y);
                pos.y = center.y;
                pos.z = center.z + Rings[j].Radius* Mathf.Cos(angle + transform.localRotation.y);

                Vector3 rotation = new Vector3(0, (angle + transform.localRotation.y) * (180 / Mathf.PI), 0);
                Transform meter = Instantiate(Prefab, pos, Quaternion.Euler(rotation), transform).transform;
                _meters.Add(meter);
                meter.GetComponent<MeshRenderer>().sharedMaterial = VisualizerMaterial;
                _meterMats.Add(meter.GetComponent<MeshRenderer>().material);
            }
        }
    }

    private void GenerateLineVisualizer()
    {
        _trueNumberOfObjects += LineVisualizer.NumberOfObjects;
        float offset = 0;

        Transform firstMeter = Instantiate(Prefab, transform.position + new Vector3((offset), 0, 0), Quaternion.identity, transform).transform;
        _meters.Add(firstMeter);
        firstMeter.GetComponent<MeshRenderer>().sharedMaterial = VisualizerMaterial;
        _meterMats.Add(firstMeter.GetComponent<MeshRenderer>().material);

        for (int i = 0; i < LineVisualizer.NumberOfObjects - 1; i++)
        {
            if (i % 2 == 0)
            {
                offset = Mathf.Abs(offset);
                offset += LineVisualizer.DistanceBetweenEachObject;
            }
            else
            {
                offset = -offset;
            }

            Transform meter = Instantiate(Prefab, transform.position + new Vector3((offset), 0, 0), Quaternion.identity, transform).transform;
            _meters.Add(meter);
            meter.GetComponent<MeshRenderer>().sharedMaterial = VisualizerMaterial;
            _meterMats.Add(meter.GetComponent<MeshRenderer>().material);
        }

    }

    private void AnalyzeSound()
    {
        AudioSource.GetOutputData(_samples, 0); // fill array with samples

        float sum = 0;
        for (int i = 0; i < qSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }

        _rms = Mathf.Sqrt(sum / qSamples); // rms = square root of average
        _dB = 20 * Mathf.Log10(_rms / refRms); // calculate dB
        if (_dB < -160) _dB = -160; // clamp dB to -160dB min
        if (_dB > _maxdB) _maxdB = _dB;
        if (_dB > _decreasingdB)
        {
            _decreasingdB = _dB;
            _decreaseRate = 0.005f;
        }
        else if (_decreasingdB > 0)
        {
            _decreasingdB -= _decreaseRate;
            _decreaseRate *= 1.2f;
        }

        AudioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
    }

    private void VisualizeSound()
    {
        if (UseBeat == true)
        {
            // Beat
            Vector3 BeatScale = BeatObject.localScale;
            float newBeatScale = _spectrum[BeatSpectrumSample] * BeatScaleHeight;
            if (newBeatScale > BeatScale.y)
            {
                BeatScale = new Vector3(newBeatScale, newBeatScale, newBeatScale);
            }
            else
            {
                BeatScale = Vector3.Lerp(BeatScale,
                    new Vector3(MinScale + _spectrum[BeatSpectrumSample] * BeatScaleHeight, MinScale + _spectrum[BeatSpectrumSample] * BeatScaleHeight,
                        MinScale + _spectrum[BeatSpectrumSample] * BeatScaleHeight),
                    Time.deltaTime * BeatScaleSpeed);
            }

            if (BeatScale.y > BeatSizeThreshold) // Apply max threshold
                BeatScale.y = BeatSizeThreshold;

            BeatObject.localScale = BeatScale;
        }

        // Main
        for (int i = 0; i < _trueNumberOfObjects; i++)
        {
            // Scale
            Vector3 scale = _meters[i].localScale;
            float newScale = _spectrum[i] * ScaleHeight;

            if (Smoothing) // Apply smoothing
            {
                scale.y = Mathf.Lerp(scale.y, _spectrum[i] * ScaleHeight, Time.deltaTime * ScaleSpeed);

                float value = Mathf.Lerp(_spectrum[i], (_spectrum[i] / _highestValues[i] * ColourMultiplier), Time.deltaTime * ColourScaleSpeed);
                _meterMats[i].SetColor("_EmissionColor", new Color(0, value, value));
            }
            else
            {
                if (newScale > scale.y)
                {
                    scale.y = newScale;

                    float value = (_spectrum[i] / _highestValues[i] * ColourMultiplier);
                    _meterMats[i].SetColor("_EmissionColor", new Color(0, value, value));
                }
                else
                {
                    scale.y = Mathf.Lerp(scale.y, _spectrum[i] * ScaleHeight, Time.deltaTime * ScaleSpeed);

                    float value = Mathf.Lerp(_meterMats[i].GetColor("_EmissionColor").g, 0, Time.deltaTime * ColourScaleSpeed);
                    _meterMats[i].SetColor("_EmissionColor", new Color(0, value, value));
                }
            }

            if (scale.y > HeightThreshold) // Apply max threshold
                scale.y = HeightThreshold;

            _meters[i].localScale = scale;

            // Position
            if (CenterVisualizer == false)
                _meters[i].localPosition = new Vector3(_meters[i].localPosition.x, transform.InverseTransformPoint(transform.position).y - 2.845f + _meters[i].localScale.y / 2, _meters[i].localPosition.z);
            else
                _meters[i].localPosition = new Vector3(_meters[i].localPosition.x, transform.InverseTransformPoint(transform.position).y, _meters[i].localPosition.z);

            // Color
            if (_spectrum[i] > _highestValues[i])
            {
                _highestValues[i] = _spectrum[i];
                if (_spectrum[i] > _highestMeter)
                    _highestMeter = _spectrum[i];
            }

            // Autothreshold
            if (AutoThreshold)
            {
                HeightThreshold = _highestMeter * ScaleHeight / 3;
            }
        }

        if (PointLight != null)
        {
            // Point Light
            if (PointLightOn == true)
            {
                PointLight.gameObject.SetActive(true);

                if (RandomizePointLight == true)
                {
                    if (_changePointLightColor == false && _decreasingdB > _maxdB - (_maxdB / PointLightRandomizerUpperThreshold))
                    {
                        _changePointLightColor = true;
                        PointLight.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
                    }
                    else if (_changePointLightColor == true && _decreasingdB < _maxdB - (_maxdB / PointLightRandomizerLowerThreshold))
                    {
                        _changePointLightColor = false;
                    }
                }
                else
                {
                    PointLight.color = PointLightStaticColor;
                }

                PointLight.intensity = (_decreasingdB / _maxdB) * PointLightIntensity;
            }
            else
            {
                PointLight.gameObject.SetActive(false);
            }
        }

        if (Spotlight != null)
        {
            // Spotlight
            if (SpotlightOn == true)
            {
                Spotlight.gameObject.SetActive(true);

                if (RandomizeSpotlight == true)
                {
                    if (_changeSpotlightColor == false && _decreasingdB > _maxdB - (_maxdB / SpotlightRandomizerUpperThreshold))
                    {
                        _changeSpotlightColor = true;
                        Spotlight.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
                    }
                    else if (_changeSpotlightColor == true && _decreasingdB < _maxdB - (_maxdB / SpotlightRandomizerLowerThreshold))
                    {
                        _changeSpotlightColor = false;
                    }
                }
                else
                {
                    Spotlight.color = SpotlightStaticColor;
                }

                Spotlight.intensity = (_decreasingdB / _maxdB) * SpotlightIntensity;
            }
            else
            {
                Spotlight.gameObject.SetActive(false);
            }
        }
    }

    private void RotateVisualizer()
    {
        transform.Rotate(Vector3.up, 6 * 1 * Time.deltaTime, Space.World);
    }

    private void ResetData()
    {
        _maxdB = InitialMaxdB;
        _highestMeter = 0;

        Array.Clear(_highestValues, 0 , _highestValues.Length);
    }
}
