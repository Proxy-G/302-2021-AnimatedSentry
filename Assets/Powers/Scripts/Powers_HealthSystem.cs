using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_HealthSystem : MonoBehaviour
{
    public float health { get; private set; }
    public float maxHealth = 100;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage){
        
        if (damage <= 0) return;

        health -= damage;

        if (health <= 0) Die();
    }

    public void Die()
    {
        //removes this gameobject from the game:
        Destroy(gameObject);
    }
}
