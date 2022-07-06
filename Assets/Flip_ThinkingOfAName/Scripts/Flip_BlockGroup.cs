using UnityEngine;

public class Flip_BlockGroup : MonoBehaviour, Flip_ISpawnable
{
    public Flip_Block[] Blocks;

    public void Spawn()
    {
        foreach (Flip_Block block in Blocks)
        {
            block.Spawn();
        }
    }
}
