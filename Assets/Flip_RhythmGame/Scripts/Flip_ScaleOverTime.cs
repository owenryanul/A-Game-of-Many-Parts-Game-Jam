using UnityEngine;

public class Flip_ScaleOverTime : MonoBehaviour
{
    public float ScaleSpeed;

    public void Update()
    {
        float scale = ScaleSpeed * Time.deltaTime;
        transform.localScale += new Vector3(scale, scale, scale);
    }
}
