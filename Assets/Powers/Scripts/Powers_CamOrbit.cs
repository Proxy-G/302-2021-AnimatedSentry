using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_CamOrbit : MonoBehaviour
{
    public Powers_PlayerMovement moveScript;
    private Powers_PlayerTargeting targetScript;
    private Camera cam;
    public float mouseSensitivityX = 4;
    public float mouseSensitivityY = 4;

    public float regDistance = 7;
    public float zoomDistance = 4;

    public float shakeIntensity = 0;

    private float yaw = 0;
    private float pitch = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        targetScript = moveScript.GetComponent<Powers_PlayerTargeting>();
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerOrbitCam();

        transform.position = moveScript.transform.position;

        //if aiming, set camera's rotation to look at target
        RotateCamToLookAtTarget();

        // "zoom" in the camera
        ZoomCamera();

        ShakeCamera();
    }

    private bool IsTargeting()
    {
        return (targetScript && targetScript.target != null && targetScript.wantsToTarget);
    }

    private void ZoomCamera()
    {
        float dis = regDistance;
        if (IsTargeting()) dis = zoomDistance;

        //check if object behind cam
        RaycastHit hit;

        //do checks to make sure no objects are behind or around the camera
        bool hitObject = Physics.Raycast(transform.position, -transform.forward, out hit, dis, LayerMask.GetMask("Default"));
        if(!hitObject) Physics.Raycast(transform.position, -transform.forward + new Vector3(1f, 0, 0), out hit, dis, LayerMask.GetMask("Default"));
        if (!hitObject) Physics.Raycast(transform.position, -transform.forward + new Vector3(-1f, 0, 0), out hit, dis, LayerMask.GetMask("Default"));
        if (!hitObject) Physics.Raycast(transform.position, -transform.forward + new Vector3(0, 1f, 0), out hit, dis, LayerMask.GetMask("Default"));
        if (!hitObject) Physics.Raycast(transform.position, -transform.forward + new Vector3(0, -1f, 0), out hit, dis, LayerMask.GetMask("Default"));

        if (hitObject) cam.transform.localPosition = Powers_AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 0, -hit.distance + .5f), 0.000001f);
        else cam.transform.localPosition = Powers_AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 0, -dis + .5f), 0.001f);
    }

    private void RotateCamToLookAtTarget()
    {
        if (IsTargeting())
        {
            //if targeting, set rotation to look at target

            Vector3 vToTarget = targetScript.target.position - cam.transform.position;
            Quaternion targetRot = Quaternion.LookRotation(vToTarget, Vector3.up);

            cam.transform.rotation = Powers_AnimMath.Slide(cam.transform.rotation, targetRot, .001f);
        }
        else
        {
            //if NOT targeting, reset rotation
            cam.transform.localRotation = Powers_AnimMath.Slide(cam.transform.localRotation, Quaternion.identity, .001f);
        }
    }

    private void PlayerOrbitCam()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * mouseSensitivityX;
        pitch += my * mouseSensitivityY;
        
        if(IsTargeting()) //z-targeting:
        {
            pitch = Mathf.Clamp(pitch, -10, 50);
            //find player yaw:
            float playerYaw = moveScript.transform.eulerAngles.y;
            //clamp camera-rig yaw to playerYaw +- 40:
            yaw = Mathf.Clamp(yaw, playerYaw - 40, playerYaw + 40);
        }
        else //not targeting/ free look:
        {
            pitch = Mathf.Clamp(pitch, -20, 89);
        }


        transform.rotation = Powers_AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }

    public void Shake(float intensity)
    {
        shakeIntensity += intensity;
        shakeIntensity = Mathf.Clamp(shakeIntensity, 0, 10);
    }

    private void ShakeCamera()
    {
        if (shakeIntensity < 0) shakeIntensity = 0;
        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime * (4 * shakeIntensity);
        else return; //shake intensity is 0, do nothing.
        
        Quaternion targetRot = Powers_AnimMath.Lerp(Random.rotation, Quaternion.identity, .999f);

        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Random.rotation, 0.001f * shakeIntensity);
    }
}
