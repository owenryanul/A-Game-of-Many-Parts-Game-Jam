using System;
using UnityEngine;

public class Flip_PlayerDamager : MonoBehaviour
{
    public Flip_PlayerDamageManager PlayerDamageManager;
    public int DamageValue;

    public void Start()
    {
        if (PlayerDamageManager == null)
        {
            PlayerDamageManager = FindObjectOfType<Flip_PlayerDamageManager>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("base_player") && !PlayerDamageManager.PlayerLogic.getIsDodging())
        {
            PlayerDamageManager.Damage(DamageValue);
        }
    }
}
