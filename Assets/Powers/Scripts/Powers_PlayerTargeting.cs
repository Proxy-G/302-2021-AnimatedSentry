using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_PlayerTargeting : MonoBehaviour
{
    public Transform target;
    public bool wantsToTarget;
    public float visionDistance = 10;

    private List<Powers_TargetableObject> potentialTargets = new List<Powers_TargetableObject>();

    float scanCooldown = 0;
    float pickCooldown = 0;

    void Start()
    {

    }

    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");

        scanCooldown -= Time.deltaTime; //count down each tick
        if (scanCooldown <= 0) ScanForTargets(); //once countdown completes, scan for targets

        pickCooldown -= Time.deltaTime; //count down each tick
        if (pickCooldown <= 0) PickTarget(); //once countdown completes, pick target
    }

    private void ScanForTargets()
    {
        scanCooldown = 1; //set cooldown timer to next scan

        potentialTargets.Clear(); //empty targets list

        //refill targets list
        Powers_TargetableObject[] objects = GameObject.FindObjectsOfType<Powers_TargetableObject>();

        foreach (Powers_TargetableObject targetObject in objects)
        {
            //check how far object is
            Vector3 disToObject = targetObject.transform.position - transform.position;

            if (disToObject.sqrMagnitude < visionDistance * visionDistance)
            {
                //check what direction the object is
                if (Vector3.Angle(transform.forward, disToObject) < 45)
                {
                    potentialTargets.Add(targetObject);
                }
            }
        }
    }

    void PickTarget()
    {
        pickCooldown = 0.25f; //set cooldown timer to next scan

        //if (target) return; //we already have a target

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
