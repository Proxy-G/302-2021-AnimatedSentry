using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_CamOrbit : MonoBehaviour
{
    public Powers_PlayerMovement target;
    public float mouseSensitivityX = 4;
    public float mouseSensitivityY = 4;

    private float yaw = 0;
    private float pitch = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateCam();

        transform.position = target.transform.position;
    }

    private void RotateCam()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * mouseSensitivityX;
        pitch += my * mouseSensitivityY;

        pitch = Mathf.Clamp(pitch, -89, 89);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
