using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_PlayerTargeting : MonoBehaviour
{
    public Transform target;
    public bool wantsToTarget;
    public float visionDistance = 10;
    public float visionAngle = 45;

    private List<Powers_TargetableObject> potentialTargets = new List<Powers_TargetableObject>();

    float scanCooldown = 0;
    float pickCooldown = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");
        if (!wantsToTarget) target = null;

        scanCooldown -= Time.deltaTime; //count down each tick
        if (scanCooldown <= 0 || (target == null && wantsToTarget)) ScanForTargets(); //once countdown completes, scan for targets

        pickCooldown -= Time.deltaTime; //count down each tick
        if (pickCooldown <= 0) PickTarget(); //once countdown completes, pick target

        // if we have target and we can't see it, set target null
        if (target && CanSeeThing(target) == false) target = null;
    }

    private bool CanSeeThing(Transform targetObject){

        if (!targetObject) return false; //error

        //check distance
        Vector3 vToThing = targetObject.position - transform.position;
        if (vToThing.sqrMagnitude > visionDistance*visionDistance) return false; //object is too far away to see

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
            if (CanSeeThing(targetObject.transform)) potentialTargets.Add(targetObject);
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
    }    
}
