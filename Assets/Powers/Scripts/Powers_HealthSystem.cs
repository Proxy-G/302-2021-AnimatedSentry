using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_HealthSystem : MonoBehaviour
{
    [HideInInspector]
    public float health;
    public float maxHealth = 100;
    public float regenSpeed = 0;

    private void Start()
    {
        health = maxHealth;
    }

    public void Update()
    {
        if(health > 0) health += regenSpeed * Time.deltaTime;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void TakeDamage(float damage){
        
        if (damage <= 0) return;

        health -= damage;

    }
}
