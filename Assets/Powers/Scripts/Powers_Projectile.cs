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
    private bool particlesSpawned = false;

    public AudioSource projectileSource;
    public MeshRenderer projectileRenderer;
    public AudioClip hitSFX;

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
        if(!particlesSpawned) //Check to make sure projectile isnt getting destroyed
        {
            //grab player movement or turret AI script
            Powers_PlayerMovement player = other.GetComponent<Powers_PlayerMovement>();

            if (player) //check if player movement script is null. if not, then we have a player
            {
                Powers_HealthSystem playerHealth = player.GetComponent<Powers_HealthSystem>();
                if (playerHealth) playerHealth.TakeDamage(damage);
                projectileSource.PlayOneShot(hitSFX);
            }

            ProjectileDestroy();
        }
    }

    private void ProjectileDestroy()
    {
        //spawn lazer hit particle system
        if(prefabLazerHit != null && !particlesSpawned) Instantiate(prefabLazerHit, transform.position, transform.rotation);
        particlesSpawned = true;
        //Disable mesh renderer if one is available.
        if(projectileRenderer != null)projectileRenderer.enabled = false;

        //destroy particle
        Destroy(gameObject, hitSFX.length);
    }
}
