using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using Note = Melanchall.DryWetMidi.Interaction.Note;

public class Flip_MidiParser : MonoBehaviour
{
    public MidiFile MidiFile;

    public AudioSource AudioSource;
    public string FileLocation;

    public Flip_MidiSpawner[] MidiSpawners;

    private void Start()
    {
        ReadFromFile();
    }

    private void ReadFromFile()
    {
        MidiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + FileLocation);
        GetDataFromMidi();
    }

    private void GetDataFromMidi()
    {
        Note[] notes = MidiFile.GetNotes().ToArray();
        foreach (Flip_MidiSpawner midiSpawner in MidiSpawners)
        {
            midiSpawner.SetTimeStamps(notes);
        }
    }

    public double GetAudioSourceTime()
    {
        return (double) AudioSource.timeSamples / AudioSource.clip.frequency;
    }
}
