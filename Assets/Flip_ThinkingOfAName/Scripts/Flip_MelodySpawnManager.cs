using System;
using UnityEngine;

public class Flip_MelodySpawnManager : MonoBehaviour
{
    [Serializable]
    public struct Note
    {
        public bool SkipBeat;
        public GameObject SpawnObject;
        public Flip_ISpawnable Spawnable;
    }

    public Note[] Notes;
    private int _noteIndex;

    public void Start()
    {
        for (var index = 0; index < Notes.Length; index++)
        {
            if (Notes[index].SkipBeat)
            {
                continue;
            }

            Notes[index].Spawnable = Notes[index].SpawnObject.GetComponent<Flip_ISpawnable>();
        }
    }

    public void Spawn()
    {
        if (_noteIndex >= Notes.Length)
        {
            return;
        }

        if (!Notes[_noteIndex].SkipBeat)
        {
            Notes[_noteIndex].Spawnable.Spawn();
        }

        _noteIndex++;
    }
}
