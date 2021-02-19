using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private CharacterController pawn;
    public float walkSpeed = 2.5f;

    public Transform legL;
    public Transform legR;
    private Vector3 inputDirection = new Vector3();
    
    /// <summary>
    /// How fast player is currently moving vertically (y-axis), in meters/sec.
    /// </summary>
    private float verticalVelocity = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        WiggleLegs();
    }

    private void MovePlayer()
    {
        float h = Input.GetAxis("Horizontal"); //Controls strafing
        float v = Input.GetAxis("Vertical"); //Controls forward/backward

        bool isTryingToMove = (h != 0 || v != 0);
        if (isTryingToMove)
        {
            //Turn to face current direction
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = Powers_AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), .01f);
        }

        inputDirection = (transform.forward * v) + (transform.right * h);
        if (inputDirection.sqrMagnitude > 1) inputDirection.Normalize();

        //apply gravity:
        verticalVelocity += 10 * Time.deltaTime;

        //adds lateral movement to vertical movement:
        Vector3 moveDelta = inputDirection * walkSpeed + verticalVelocity * Vector3.down;

        //move pawn:
        CollisionFlags flags = pawn.Move(moveDelta * Time.deltaTime);

        if(pawn.isGrounded)
        {
            verticalVelocity = 0; //on ground, zero-out y-velocity:
        }
    }

    private void WiggleLegs()
    {
        float degrees = 45;
        float speed = 10;

        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDirection);
        Vector3 axis = Vector3.Cross(inputDirLocal, Vector3.up);

        //check  alignment of inputDirLocal against forward vector
        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        
        alignment = Mathf.Abs(alignment); //flips negative numbers
        degrees *= Powers_AnimMath.Lerp(0.25f, 1f, alignment, true); //decrease degrees when strafing

        float wave = Mathf.Sin(Time.time * speed) * degrees;

        legL.localRotation = Powers_AnimMath.Slide(legL.localRotation, Quaternion.AngleAxis(wave, axis), 0.001f);
        legR.localRotation = Powers_AnimMath.Slide(legR.localRotation, Quaternion.AngleAxis(-wave, axis), 0.001f);
    }
}
