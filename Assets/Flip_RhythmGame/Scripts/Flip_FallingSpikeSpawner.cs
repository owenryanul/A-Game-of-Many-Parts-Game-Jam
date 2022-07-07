using UnityEngine;
using Random = UnityEngine.Random;

public class Flip_FallingSpikeSpawner : MonoBehaviour, Flip_ISpawnable
{
    public GameObject FallingSpike;
    public Vector2 SpawnPositionMin;
    public Vector2 SpawnPositionMax;

    public void Spawn()
    {
        Vector2 position = new Vector2(Random.Range(SpawnPositionMin.x, SpawnPositionMax.x), Random.Range(SpawnPositionMin.y, SpawnPositionMax.y));
        Flip_FallingSpike spike = Instantiate(FallingSpike, position, Quaternion.Euler(0, 0, 180)).GetComponent<Flip_FallingSpike>();
        spike.StartPosition = position;
        spike.EndPosition = new Vector2(position.x, 4.9f);
        spike.Spawn();
    }
}
