using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_PlayerTargeting : MonoBehaviour
{
    public Transform target;
    public Powers_CamOrbit camOrbit;
    public float visionDistance = 10;
    public float visionAngle = 45;

    [HideInInspector]
    public bool wantsToTarget;
    private bool wantsToAttack;

    //references player arm bones
    [Space(10)]
    public Transform armL;
    public Transform armR;

    private Vector3 startPosArmL;
    private Vector3 startPosArmR;

    [Space(10)]
    /// <summary>
    /// references particle system for gun muzzle flash
    /// </summary>
    public ParticleSystem prefabMuzzleFlash;
    public Transform pistolL;
    public Transform pistolR;
    public AudioClip pistolShot;
    private AudioSource pistolLSource;
    private AudioSource pistolRSource;
    private bool pistolLNext = true;

    private List<Powers_TargetableObject> potentialTargets = new List<Powers_TargetableObject>();

    float scanCooldown = 0;
    float pickCooldown = 0;
    float shootCooldown = 0;

    public float roundPerSecond = 6;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        startPosArmL = armL.localPosition;
        startPosArmR = armR.localPosition;

        pistolLSource = pistolL.GetComponent<AudioSource>();
        pistolRSource = pistolR.GetComponent<AudioSource>();
    }

    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");
        wantsToAttack = Input.GetButton("Fire1");

        if (!wantsToTarget) target = null;

        scanCooldown -= Time.deltaTime; //count down each tick
        if (scanCooldown <= 0 || (target == null && wantsToTarget)) ScanForTargets(); //once countdown completes, scan for targets

        pickCooldown -= Time.deltaTime; //count down each tick
        if (pickCooldown <= 0) PickTarget(); //once countdown completes, pick target

        shootCooldown -= Time.deltaTime; //count down each tick

        // if we have target and we can't see it, set target null
        if (target && CanSeeThing(target) == false) target = null;

        SlideArmsHome();
        DoAttack(); //once countdown completes, pick target
    }

    private bool CanSeeThing(Transform targetObject) {

        if (!targetObject) return false; //error

        //check distance
        Vector3 vToThing = targetObject.position - transform.position;
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; //object is too far away to see

        //check direction
        if (Vector3.Angle(transform.forward, vToThing) > visionAngle) return false; //outside cone of vision

        //TODO: check occlusion

        return true; //object visible!
    }

    private void ScanForTargets()
    {
        scanCooldown = 1; //set cooldown timer to next scan

        potentialTargets.Clear(); //empty targets list

        //refill targets list
        Powers_TargetableObject[] objects = GameObject.FindObjectsOfType<Powers_TargetableObject>();


        foreach (Powers_TargetableObject targetObject in objects)
        {
            //if we can see it, add to potential targets
            if (CanSeeThing(targetObject.transform) && targetObject.enabled) potentialTargets.Add(targetObject);
        }
    }

    void PickTarget()
    {
        pickCooldown = 0.25f; //set cooldown timer to next scan

        //if (target) return; //we already have a target
        target = null;

        float closestTargetDistance = 0;

        // find closest targetable object and sets it as the target
        foreach (Powers_TargetableObject pt in potentialTargets)
        {
            float dd = (pt.transform.position - transform.position).sqrMagnitude;
            if (dd < closestTargetDistance || target == null)
            {
                target = pt.transform;
                closestTargetDistance = dd;
            }

        }

        //Remove targetable objects that are now null or disabled.
        for (int i = potentialTargets.Count - 1; i >= 0; i--)
        {
            if (potentialTargets[i] == null) potentialTargets.RemoveAt(i);
            else if (!potentialTargets[i].enabled) potentialTargets.RemoveAt(i);
        }
    }

    private void DoAttack()
    {
        if (shootCooldown > 0) return; //too soon
        if (!wantsToTarget) return; //player not targeting
        if (!wantsToAttack) return; //player not shooting
        if (target == null) return; //no target available
        if (!CanSeeThing(target)) return; //target not in sight

        //set cooldown
        shootCooldown = 1 / roundPerSecond;

        Powers_HealthSystem targetHealth = target.GetComponent<Powers_HealthSystem>();

        if(targetHealth)
        {
            targetHealth.TakeDamage(Random.Range(18, 25));
            //if target has been killed, add one to the kill count.
        }

        //attack!

        if (pistolL && pistolLNext) {
            Instantiate(prefabMuzzleFlash, pistolL.position, pistolL.rotation);
            pistolLSource.PlayOneShot(pistolShot);
        }
        if (pistolR && !pistolLNext) {
            Instantiate(prefabMuzzleFlash, pistolR.position, pistolR.rotation);
            pistolRSource.PlayOneShot(pistolShot);
        }

        camOrbit.Shake(2);

        //trigger arm anim

        //rotates arms up:
        if(pistolLNext) armL.localEulerAngles += new Vector3(-15, 0, 0);
        else armR.localEulerAngles += new Vector3(-15, 0, 0);

        //move arms back:
        if (pistolLNext) armL.position += -armL.transform.forward * .05f;
        else armR.position += -armR.transform.forward * .05f;

        pistolLNext = !pistolLNext;
    }

    private void SlideArmsHome()
    {
        armL.localPosition = Powers_AnimMath.Slide(armL.localPosition, startPosArmL, 0.02f);
        armR.localPosition = Powers_AnimMath.Slide(armR.localPosition, startPosArmR, 0.02f);
    }
}
