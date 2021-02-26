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

    public float gravityMultiplier = 10;
    public float jumpImpulse = 5;

    private float timeLeftGrounded = 0;
    private bool jumpAnimLegsSwitch = true;
    private bool jumpFirst = true;

    public bool isGrounded
    {
        get { //return true if pawn is on ground OR "coyote-time" is not zero
            return pawn.isGrounded || timeLeftGrounded > 0;
        }
    }

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
        //coyote time countdown:
        if (timeLeftGrounded > 0) timeLeftGrounded -= Time.deltaTime;
        
        MovePlayer();
        if (pawn.isGrounded) WiggleLegs(); //idle + walk
        else AirLegs(); //jump + falling
    }

    private void MovePlayer()
    {
        float h = Input.GetAxis("Horizontal"); //Controls strafing
        float v = Input.GetAxis("Vertical"); //Controls forward/backward

        bool isJumpHeld = Input.GetButton("Jump");
        bool onJumpPress = Input.GetButtonDown("Jump");

        bool isTryingToMove = (h != 0 || v != 0);
        if (isTryingToMove)
        {
            //Turn to face correct direction
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = Powers_AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), .01f);
        }

        inputDirection = (transform.forward * v) + (transform.right * h);
        if (inputDirection.sqrMagnitude > 1) inputDirection.Normalize();

        //apply gravity:
        verticalVelocity += gravityMultiplier * Time.deltaTime;

        //adds lateral movement to vertical movement:
        Vector3 moveDelta = inputDirection * walkSpeed + verticalVelocity * Vector3.down;

        //move pawn:
        CollisionFlags flags = pawn.Move(moveDelta * Time.deltaTime);
        if(pawn.isGrounded)
        {
            verticalVelocity = 0; //on ground, zero-out vertical-velocity
            timeLeftGrounded = .1f;
            jumpFirst = true;
        }

        if (isGrounded) {
            if (isJumpHeld) {
                verticalVelocity = -jumpImpulse;
            }
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

    private void AirLegs()
    {
        if(jumpFirst) jumpAnimLegsSwitch = !jumpAnimLegsSwitch;
        jumpFirst = false;

        if (jumpAnimLegsSwitch)
        {
            legL.localRotation = Powers_AnimMath.Slide(legL.localRotation, Quaternion.Euler(30, 0, 0), 0.05f);
            legR.localRotation = Powers_AnimMath.Slide(legR.localRotation, Quaternion.Euler(-30, 0, 0), 0.05f);
        }
        else
        {
            legL.localRotation = Powers_AnimMath.Slide(legL.localRotation, Quaternion.Euler(-30, 0, 0), 0.05f);
            legR.localRotation = Powers_AnimMath.Slide(legR.localRotation, Quaternion.Euler(30, 0, 0), 0.05f);
        }
    }    
}
