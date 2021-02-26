using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_Projectile : MonoBehaviour
{
    public float lifetime = 10;
    public ParticleSystem prefabLazerHit;

    private void Update()
    {
        //countdown until lazer is automatically destroyed
        lifetime -= Time.deltaTime;

        transform.position += (-transform.forward * 8 * Time.deltaTime);

        if (lifetime <= 0) ProjectileDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {        
        //grab player movement script
        Powers_PlayerMovement player = other.GetComponent<Powers_PlayerMovement>();
        //check if player movement script is null. if not, then we have a player
        if (player)
        {
            Powers_HealthSystem playerHealth = player.GetComponent<Powers_HealthSystem>();
            if (playerHealth) playerHealth.TakeDamage(10);
        }

        ProjectileDestroy();
    }

    private void ProjectileDestroy()
    {
        //spawn lazer hit particle system
        Instantiate(prefabLazerHit, transform.position, transform.rotation);

        //destroy particle
        Destroy(gameObject);
    }
}
