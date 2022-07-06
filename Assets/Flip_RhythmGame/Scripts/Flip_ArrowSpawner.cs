using System.Linq;
using UnityEngine;

public class Flip_ArrowSpawner : MonoBehaviour
{
    public GameObject Arrow;
    public Vector2 SpawnPositionRangeMin;
    public Vector2 SpawnPositionRangeMax;
    public Vector2 SpawnVelocityRangeMin;
    public Vector2 SpawnVelocityRangeMax;

    public GameObject ArrowWithGravity;
    public Vector2 ArrowExplosionPosition;
    public Vector2 ExplosionVelocityRangeMin;
    public Vector2 ExplosionVelocityRangeMax;
    public float SpawnTorqueRangeMin;
    public float SpawnTorqueRangeMax;

    public int[] ExplosionIndexes;
    private int _currentIndex;

    public void Spawn()
    {
        if (ExplosionIndexes.Contains(_currentIndex))
        {
            SpawnArrowExplosion();
        }
        else
        {
            SpawnArrow();
        }

        _currentIndex++;
    }

    public void SpawnArrow()
    {
        Rigidbody2D arrow = Instantiate(Arrow,
            new Vector3(Random.Range(SpawnPositionRangeMin.x, SpawnPositionRangeMax.x), Random.Range(SpawnPositionRangeMin.y, SpawnPositionRangeMax.y), 0),
            Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)))).GetComponent<Rigidbody2D>();

        arrow.AddForce(new Vector2(Random.Range(SpawnVelocityRangeMin.x, SpawnVelocityRangeMax.x), Random.Range(SpawnVelocityRangeMin.y, SpawnVelocityRangeMax.y)), ForceMode2D.Force);
    }

    public void SpawnArrowExplosion()
    {
        for (int i = 0; i < 10; i++)
        {
            Rigidbody2D arrow = Instantiate(ArrowWithGravity,
                ArrowExplosionPosition,
                Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)))).GetComponent<Rigidbody2D>();

            arrow.AddForce(new Vector2(Random.Range(ExplosionVelocityRangeMin.x, ExplosionVelocityRangeMax.x), Random.Range(ExplosionVelocityRangeMin.y, ExplosionVelocityRangeMax.y)), ForceMode2D.Force);
            arrow.AddTorque(Random.Range(SpawnTorqueRangeMin, SpawnTorqueRangeMax), ForceMode2D.Force);
        }
    }
}
