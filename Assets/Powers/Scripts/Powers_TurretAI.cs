using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Powers_TurretAI : MonoBehaviour
{
    public Powers_PlayerMovement playerMovementScript;

    [Space(10)]
    public Transform shootPosL;
    public Transform shootPosR;
    public GameObject prefabLazerProjectile;

    [Space(10)]
    public AudioSource turretSource;
    public AudioClip lazerShootSFX;
    public AudioClip turretDieSFX;

    [HideInInspector]
    public Transform playerPos;

    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public bool reachedPlayer = false;
    [HideInInspector]
    public bool canAttack = false;

    [HideInInspector]
    public float shootCooldown = 0.4f;
    [HideInInspector]
    public bool nextShotL = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerPos = playerMovementScript.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveAI();

        if (canAttack) AttackAI();
    }

    private void MoveAI()
    {
        //Move the agent
        agent.destination = playerPos.position;

        //If the agent has reached the player, allow the turret to prepare attack
        if (Vector3.Distance(transform.position, playerPos.position) <= agent.stoppingDistance + 0.1f) reachedPlayer = true;
        else reachedPlayer = false;

    }

    private void AttackAI()
    {
        shootCooldown -= Time.deltaTime; //countdown

        //if countdown is complete, shoot a lazer at the player.
        if(shootCooldown < 0)
        {
            //If the next shot is from wing L, create lazer from wing L. Else, create lazer from wing R.
            if(nextShotL) Instantiate(prefabLazerProjectile, shootPosL.position, shootPosL.rotation);
            else Instantiate(prefabLazerProjectile, shootPosR.position, shootPosR.rotation);
            turretSource.PlayOneShot(lazerShootSFX);

            //Flip nextShotL so the next shot comes from opposite wing.
            nextShotL = !nextShotL;

            //Reset shoot cooldown
            shootCooldown = 0.4f;
        }
    }
}
