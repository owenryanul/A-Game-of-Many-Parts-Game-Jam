using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Visualizer : MonoBehaviour
{
    [Serializable]
    public struct Ring
    {
        public int NumberOfObjects;
        public float Radius;
    }

    public GameObject prefab;
    public Light pointLight;
    public Light spotlight;

    [Header("Starting Parameters")]
    public Ring[] Rings;
    private int _trueNumberOfObjects;
    public AudioSource AudioSource;

    [Tooltip("maxdB buffer")]
    public float initialMaxdB;

    [Header("Visualizer Properties")]
    public float scale;
    public bool centerVisualizer;
    public bool rotate;

    [Header("Bar Properties")]
    public bool autoThreshold;
    public float heightThreshold;
    public float scaleSpeed;
    public float colourScaleSpeed;
    public float colourMultiplier;
    public float scaleHeight;
    public bool smoothing;

    [Header("Beat Properties")]
    public Transform BeatObject;
    public int BeatSpectrumSample;
    public float BeatScaleHeight;
    public float BeatSizeThreshold;
    public float MinScale;
    public float BeatScaleSpeed;

    [Header("Point Light Properties")]
    public bool pointLightOn;
    public bool randomizePointLight;
    [Tooltip("dB > maxdB - (maxdB / upperThreshold) to change color")]
    public float pointLightRandomizerUpperThreshold;
    [Tooltip("dB < maxdB - (maxdB / lowerThreshold) to allow another change color")]
    public float pointLightRandomizerLowerThreshold;
    public Color pointLightStaticColor;
    public float pointLightIntensity;

    [Header("Spotlight Properties")]
    public bool spotlightOn;
    public bool randomizeSpotlight;
    [Tooltip("dB > maxdB - (maxdB / upperThreshold) to change color")]
    public float spotlightRandomizerUpperThreshold;
    [Tooltip("dB < maxdB - (maxdB / lowerThreshold) to allow another change color")]
    public float spotlightRandomizerLowerThreshold;
    public Color spotlightStaticColor;
    public float spotlightIntensity;

    private List<Transform> meters = new List<Transform>();
    private List<Material> meterMats = new List<Material>();

    private float rms; // sound level - RMS
    private float dB; // sound level - dB
    private float maxdB;
    private float decreasingdB = 0;
    private float decreaseRate = 0.005f;

    private const int qSamples = 1024; // array size
    private const float refRMS = 0.1f; // RMS value for 0 dB

    private float[] samples;
    private float[] spectrum;
    private float[] highestValues;

    private bool changePointLightColor = false;
    private bool changeSpotlightColor = false;


    public Material visualizerMaterial;

    private float highestMeter = 0;

    void Start() {
        maxdB = initialMaxdB;

        samples = new float[qSamples];
        spectrum = new float[qSamples];
        highestValues = new float[qSamples];

        GenerateCircleVisualizer();
    }

    void FixedUpdate() {
        transform.localScale = new Vector3(scale, 1, scale);

        AnalyzeSound();
        VisualizeSound();

        if (rotate == true)
            RotateVisualizer();
    }

    void GenerateCircleVisualizer()
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
                Transform meter = Instantiate(prefab, pos, Quaternion.Euler(rotation), transform).transform;
                meters.Add(meter);
                meter.GetComponent<MeshRenderer>().sharedMaterial = visualizerMaterial;
                meterMats.Add(meter.GetComponent<MeshRenderer>().material);
            }
        }
    }

    void AnalyzeSound()
    {
        AudioSource.GetOutputData(samples, 0); // fill array with samples

        float sum = 0;
        for (int i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i]; // sum squared samples
        }

        rms = Mathf.Sqrt(sum / qSamples); // rms = square root of average
        dB = 20 * Mathf.Log10(rms / refRMS); // calculate dB
        if (dB < -160) dB = -160; // clamp dB to -160dB min
        if (dB > maxdB) maxdB = dB;
        if (dB > decreasingdB)
        {
            decreasingdB = dB;
            decreaseRate = 0.005f;
        }
        else if (decreasingdB > 0)
        {
            decreasingdB -= decreaseRate;
            decreaseRate *= 1.2f;
        }

        AudioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    }

    void VisualizeSound()
    {
        // Beat
        Vector3 BeatScale = BeatObject.localScale;
        float newBeatScale = spectrum[BeatSpectrumSample] * BeatScaleHeight;
        if (newBeatScale > BeatScale.y)
        {
            BeatScale = new Vector3(newBeatScale, newBeatScale, newBeatScale);
        }
        else
        {
            BeatScale = Vector3.Lerp(BeatScale,
                new Vector3(MinScale + spectrum[BeatSpectrumSample] * BeatScaleHeight, MinScale + spectrum[BeatSpectrumSample] * BeatScaleHeight, MinScale + spectrum[BeatSpectrumSample] * BeatScaleHeight),
                Time.deltaTime * BeatScaleSpeed);
        }

        if (BeatScale.y > BeatSizeThreshold) // Apply max threshold
            BeatScale.y = BeatSizeThreshold;

        BeatObject.localScale = BeatScale;

        // Main
        for (int i = 0; i < _trueNumberOfObjects; i++)
        {
            // Scale
            Vector3 scale = meters[i].localScale;
            float newScale = spectrum[i] * scaleHeight;

            if (smoothing) // Apply smoothing
            {
                scale.y = Mathf.Lerp(scale.y, spectrum[i] * scaleHeight, Time.deltaTime * scaleSpeed);

                float value = Mathf.Lerp(spectrum[i], (spectrum[i] / highestValues[i] * colourMultiplier), Time.deltaTime * colourScaleSpeed);
                meterMats[i].SetColor("_EmissionColor", new Color(0, value, value));
            }
            else
            {
                if (newScale > scale.y)
                {
                    scale.y = newScale;

                    float value = (spectrum[i] / highestValues[i] * colourMultiplier);
                    meterMats[i].SetColor("_EmissionColor", new Color(0, value, value));
                }
                else
                {
                    scale.y = Mathf.Lerp(scale.y, spectrum[i] * scaleHeight, Time.deltaTime * scaleSpeed);

                    float value = Mathf.Lerp(meterMats[i].GetColor("_EmissionColor").g, 0, Time.deltaTime * colourScaleSpeed);
                    meterMats[i].SetColor("_EmissionColor", new Color(0, value, value));
                }
            }

            if (scale.y > heightThreshold) // Apply max threshold
                scale.y = heightThreshold;

            meters[i].localScale = scale;

            // Position
            if (centerVisualizer == false)
                meters[i].localPosition = new Vector3(meters[i].localPosition.x, transform.InverseTransformPoint(transform.position).y - 2.845f + meters[i].localScale.y / 2, meters[i].localPosition.z);
            else
                meters[i].localPosition = new Vector3(meters[i].localPosition.x, transform.InverseTransformPoint(transform.position).y, meters[i].localPosition.z);

            // Color
            if (spectrum[i] > highestValues[i])
            {
                highestValues[i] = spectrum[i];
                if (spectrum[i] > highestMeter)
                    highestMeter = spectrum[i];
            }

            // Autothreshold
            if (autoThreshold)
            {
                heightThreshold = highestMeter * scaleHeight / 3;
            }
        }

        if (pointLight != null)
        {
            // Point Light
            if (pointLightOn == true)
            {
                pointLight.gameObject.SetActive(true);

                if (randomizePointLight == true)
                {
                    if (changePointLightColor == false && decreasingdB > maxdB - (maxdB / pointLightRandomizerUpperThreshold))
                    {
                        changePointLightColor = true;
                        pointLight.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
                    }
                    else if (changePointLightColor == true && decreasingdB < maxdB - (maxdB / pointLightRandomizerLowerThreshold))
                    {
                        changePointLightColor = false;
                    }
                }
                else
                {
                    pointLight.color = pointLightStaticColor;
                }

                pointLight.intensity = (decreasingdB / maxdB) * pointLightIntensity;
            }
            else
            {
                pointLight.gameObject.SetActive(false);
            }
        }

        if (spotlight != null)
        {
            // Spotlight
            if (spotlightOn == true)
            {
                spotlight.gameObject.SetActive(true);

                if (randomizeSpotlight == true)
                {
                    if (changeSpotlightColor == false && decreasingdB > maxdB - (maxdB / spotlightRandomizerUpperThreshold))
                    {
                        changeSpotlightColor = true;
                        spotlight.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
                    }
                    else if (changeSpotlightColor == true && decreasingdB < maxdB - (maxdB / spotlightRandomizerLowerThreshold))
                    {
                        changeSpotlightColor = false;
                    }
                }
                else
                {
                    spotlight.color = spotlightStaticColor;
                }

                spotlight.intensity = (decreasingdB / maxdB) * spotlightIntensity;
            }
            else
            {
                spotlight.gameObject.SetActive(false);
            }
        }
    }

    void RotateVisualizer()
    {
        transform.Rotate(Vector3.up, 6 * 1 * Time.deltaTime, Space.World);
    }

    void ResetData()
    {
        maxdB = initialMaxdB;
        highestMeter = 0;

        Array.Clear(highestValues, 0 , highestValues.Length);
    }
}
