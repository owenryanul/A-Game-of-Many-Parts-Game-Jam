using UnityEngine;

public class Flip_Despawner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("sample_blastable"))
        {
            Destroy(collision.gameObject);
        }
    }
}
