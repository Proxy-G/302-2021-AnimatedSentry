using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_Projectile : MonoBehaviour
{
    public bool infiniteLife = false;
    public float lifetime = 10;
    public float damage = 0;
    public float velocity = 0;
    public ParticleSystem prefabLazerHit;

    private void Update()
    {
        //countdown until lazer is automatically destroyed
        if(!infiniteLife) lifetime -= Time.deltaTime;

        transform.position += (-transform.forward * velocity * Time.deltaTime);

        //If lifetime is complete, destroy the projectile.
        if (lifetime <= 0 && !infiniteLife) ProjectileDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {        
        //grab player movement script
        Powers_PlayerMovement player = other.GetComponent<Powers_PlayerMovement>();
        //check if player movement script is null. if not, then we have a player
        if (player)
        {
            Powers_HealthSystem playerHealth = player.GetComponent<Powers_HealthSystem>();
            if (playerHealth) playerHealth.TakeDamage(damage);
        }

        ProjectileDestroy();
    }

    private void ProjectileDestroy()
    {
        //spawn lazer hit particle system
        if(prefabLazerHit != null) Instantiate(prefabLazerHit, transform.position, transform.rotation);

        //destroy particle
        Destroy(gameObject);
    }
}
