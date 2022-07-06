using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;
using UnityEngine.Events;

public class Flip_MidiSpawner : MonoBehaviour
{
    public Flip_MidiParser MidiParser;
    public NoteName NoteRestriction;
    public float PreSpawnTime;

    public UnityEvent SpawnEvent;

    private List<double> _timeStamps = new List<double>();
    private int _spawnIndex;

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == NoteRestriction)
            {
                MetricTimeSpan metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, MidiParser.MidiFile.GetTempoMap());
                _timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    public void Update()
    {
        if (_spawnIndex >= _timeStamps.Count)
        {
            return;
        }

        if (!(MidiParser.GetAudioSourceTime() >= _timeStamps[_spawnIndex] - PreSpawnTime))
        {
            return;
        }

        SpawnEvent.Invoke();
        _spawnIndex++;
    }
}
