using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_PointAt : MonoBehaviour
{
    private Powers_PlayerTargeting playerTargeting;

    private Quaternion startingRotation;

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    // Start is called before the first frame update
    void Start()
    {
        playerTargeting = GetComponentInParent<Powers_PlayerTargeting>();
        startingRotation = transform.localRotation; //Get starting rotation
    }

    // Update is called once per frame
    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if (playerTargeting && playerTargeting.target && playerTargeting.wantsToTarget)
        {
            Vector3 disToTarget = playerTargeting.target.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

            Vector3 euler1 = transform.localEulerAngles; //Get local angles BEFORE target rotation
            Quaternion prevRot = transform.rotation; //Save normal rotation
            transform.rotation = targetRotation; //Set rotation
            Vector3 euler2 = transform.localEulerAngles; //Get local angles AFTER target rotation

            if (lockRotationX) euler2.x = euler1.x; //Revert X to prev value
            if (lockRotationY) euler2.y = euler1.y; //Revert Y to prev value
            if (lockRotationZ) euler2.z = euler1.z; //Revert Z to prev value

            transform.rotation = prevRot; //revert to normal rotation
            transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), 0.02f); //animate rotation
        }
        else
        {
            //figure out bone rotation, no target:
            transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRotation, 0.04f); ;
        }
    }
}
