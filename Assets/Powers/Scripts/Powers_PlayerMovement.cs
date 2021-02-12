using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private CharacterController pawn;
    public float walkSpeed = 2.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal"); //Controls strafing
        float v = Input.GetAxis("Vertical"); //Controls forward/backward

        bool isTryingToMove = (h != 0 || v != 0);
        if(isTryingToMove)
        {
            //Turn to face current direction
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = Powers_AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), .01f);
        }

        Vector3 inputDirection = (transform.forward * v) + (transform.right * h);

        pawn.SimpleMove(inputDirection * walkSpeed);

    }
}
