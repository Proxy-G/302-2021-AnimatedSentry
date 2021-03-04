using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_TurretAnimation : MonoBehaviour
{
    public Powers_TurretAI turretAI;
    public Transform turretBody;
    public Transform turretTop;
    [Space(10)]
    public MeshRenderer turretEye;
    public Material normEyeMat;
    public Material attackEyeMat;
    public Material deadEyeMat;
    [Space(10)]
    public Transform turretWingL;
    public Transform turretWingR;
    [Space(10)]
    public Transform turretFrontLegL1;
    public Transform turretFrontLegR1;
    public Transform turretBackLegL1;
    public Transform turretBackLegR1;
    [Space(10)]
    public Transform turretFrontLegL2;
    public Transform turretFrontLegR2;
    public Transform turretBackLegL2;
    public Transform turretBackLegR2;

    private Powers_HealthSystem turretHealth;
    private Powers_HealthSystem playerHealth;

    public float radianDivide = 70;

    private bool localReachPlayer = false;
    private float localShootCooldown = 0;

    private bool turretAimAnim = false;
    private float turretHopWave = 0;
    private float turretHopTime = 0;

    private float turretWalkWave1 = 0;
    private float turretWalkWave2 = 0;

    private float turretAttackCooldown = 0.05f;
    Quaternion targetRot;


    private float wingAnimOffset = 0.15f;


    // Start is called before the first frame update
    void Start()
    {
        turretHealth = GetComponent<Powers_HealthSystem>();
        playerHealth = turretAI.playerMovementScript.GetComponent<Powers_HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //First, check if turret is dead. If so, play death anim. If not, allow normal behaviour.
        if (turretHealth.health == 0) Death();
        else
        {
            //This function is used to update and check variables.
            VariableChecks();

            //animate the body
            BodyAnim();

            //animate the wings
            WingsAnim();

            //animate the legs
            LegsAnim();

            //Set local variable references to Turret AI's variables.
            localReachPlayer = turretAI.reachedPlayer;
            localShootCooldown = turretAI.shootCooldown;
        }
    }

    private void VariableChecks()
    {
        //This is used for the wave for walking.
        turretWalkWave1 = Mathf.Sin(Time.time * 10);
        turretWalkWave2 = Mathf.Cos(Time.time * 20);

        //If the variable used to indicate a turret has reached the player changed, reset variables for the aiming animation.
        if (localReachPlayer != turretAI.reachedPlayer)
        {
            turretAimAnim = true;
            turretHopWave = 0;
            turretHopTime = 0;
        }

        //If turret aim start is true, then the turret will do a little hop while the wings open.
        if (turretHopWave >= 0 && turretAimAnim)
        {
            turretHopTime += Time.deltaTime;
            turretHopWave = Mathf.Sin(turretHopTime * 10);
        }
        if (turretHopWave < 0) turretAimAnim = false;

        //If the turret shoot cooldown has been reset and is more than the local, then reset the variables for the time for turret wing to pop back.
        if (localShootCooldown < turretAI.shootCooldown) turretAttackCooldown = 0.1f;
    }
    private void BodyAnim()
    {
        if(turretAI.canAttack) //If the turret can attack, it will prepare the attack anims.
        {
            LookAtPlayer();

            //Shrink eye with attacking
            turretEye.transform.localScale = Powers_AnimMath.Slide(turretEye.transform.localScale, new Vector3(0.1f, 0.1f, 0.1f), 0.00001f);
        }
        if (turretAI.reachedPlayer && playerHealth.health != 0) //If the turret has reached the player, it will play the aim animation.
        {
            turretEye.transform.localScale = Powers_AnimMath.Slide(turretEye.transform.localScale, new Vector3(0.2f, 0.2f, 0.2f), 0.00001f);
            turretEye.material = attackEyeMat;
            LookAtPlayer();

            //If turret aim start is true, then the turret will do a little hop while the wings open.
            if (turretHopWave >= 0 && turretAimAnim) turretBody.localPosition = Powers_AnimMath.Slide(turretBody.localPosition, new Vector3(0, -turretHopWave * .075f + 0.5f, 0), 0.001f);

            //Once the little hop is complete, then the aim start var will be disabled, and the turret body position will be reset.
            if (turretHopWave < 0)
            {
                turretBody.localPosition = Powers_AnimMath.Slide(turretBody.localPosition, new Vector3(0, 0.5f, 0), 0.001f);
            }
        }
        else if(turretAI.agent.velocity.magnitude > 0.1f) //If the turret is walking, then it will do the walk bob animation.
        {
            turretEye.material = normEyeMat;

            turretBody.localPosition = Powers_AnimMath.Slide(turretBody.localPosition, new Vector3(0, turretWalkWave2 * .025f + 0.525f, 0), 0.001f);
            turretTop.localRotation = Powers_AnimMath.Slide(turretTop.localRotation, Quaternion.Euler(0, 0, 0), 0.01f);
        }
    }

    private void WingsAnim()
    {
        if (turretAI.canAttack && playerHealth.health != 0) //If the turret can attack, it will prepare the attack anims.
        {
            if(turretAttackCooldown > 0) //Depending on which shot is next from the turret, that appropriate wing will recoil back.
            {
                if (!turretAI.nextShotL && turretAttackCooldown > 0) turretWingL.localRotation = Powers_AnimMath.Slide(turretWingL.localRotation, Quaternion.Euler(70, 25, 0), 0.00001f);
                else turretWingR.localRotation = Powers_AnimMath.Slide(turretWingR.localRotation, Quaternion.Euler(-70, 25, 0), 0.00001f);
            }
            if (turretAttackCooldown <= 0) //Once attack cooldown is complete, reset wing rotations.
            {
                turretWingL.localRotation = Powers_AnimMath.Slide(turretWingL.localRotation, Quaternion.Euler(70, 0, 0), 0.01f);
                turretWingR.localRotation = Powers_AnimMath.Slide(turretWingR.localRotation, Quaternion.Euler(-70, 0, 0), 0.01f);
            }
        }
        if (turretAI.reachedPlayer && playerHealth.health != 0) //If the turret has reached the player, it will play the aim animation.
        {
            //If the ai has switched modes within the last tick, then reset wingAnimOffset.
            if (localReachPlayer != turretAI.reachedPlayer) wingAnimOffset = 0.1f;
            
            turretWingL.localRotation = Powers_AnimMath.Slide(turretWingL.localRotation, Quaternion.Euler(70, 0, 0), 0.001f);
            // If wingAnimOffset reaches 0, begin animaitng turretWingR. This is used to offset it from turretWingL, 
            // in order to make the anim slightly asymmetrical and more interesting.
            if(wingAnimOffset <= 0) turretWingR.localRotation = Powers_AnimMath.Slide(turretWingR.localRotation, Quaternion.Euler(-70, 0, 0), 0.001f);
            else wingAnimOffset -= Time.deltaTime;

            //IMPORTANT!!!! If the anim is complete, then tell the turret AI that it is targeting the player and can attack.
            if (turretWingL.localEulerAngles.x >= 69 && turretWingR.localEulerAngles.x <= 291) turretAI.canAttack = true;
            else turretAI.canAttack = false;
        }
        else
        {
            //If the ai has switched modes within the last tick, then reset wingAnimOffset.
            if (localReachPlayer != turretAI.reachedPlayer) wingAnimOffset = 0.1f;

            turretWingL.localRotation = Powers_AnimMath.Slide(turretWingL.localRotation, Quaternion.Euler(0, 0, 0), 0.001f);
            // If wingAnimOffset reaches 0, begin animaitng turretWingR. This is used to offset it from turretWingL, 
            // in order to make the anim slightly asymmetrical and more interesting.
            if (wingAnimOffset <= 0) turretWingR.localRotation = Powers_AnimMath.Slide(turretWingR.localRotation, Quaternion.Euler(0, 0, 0), 0.001f);
            else wingAnimOffset -= Time.deltaTime;

            //Tell turret that it cannot attack the player now.
            turretAI.canAttack = false;
        }
    }

    private void LegsAnim()
    {
        //If the turret has reached the player, it will play the aim animation.
        if (turretAI.reachedPlayer && playerHealth.health != 0)
        {
            //These are used to animate the legs during the little hop while the turret aims.
            turretFrontLegL1.localRotation = Powers_AnimMath.Slide(turretFrontLegL1.localRotation, Quaternion.Euler(turretHopWave * 12.5f, turretHopWave * 12.5f, 0), 0.001f);
            turretFrontLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
            turretBackLegL1.localRotation = new Quaternion(turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
            turretBackLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);

            turretFrontLegL2.localRotation = Powers_AnimMath.Slide(turretFrontLegL2.localRotation, Quaternion.Euler(15 - turretHopWave * 10, 15 - turretHopWave * 10, 0), 0.001f);
            turretFrontLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
            turretBackLegL2.localRotation = new Quaternion(turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
            turretBackLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);

            if (turretHopWave < 0)
            {
                //This resets the leg positions once the turret is done doing the hop.
                turretFrontLegL1.localRotation = Powers_AnimMath.Slide(turretFrontLegL1.localRotation, Quaternion.identity, 0.01f);
                turretFrontLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
                turretBackLegL1.localRotation = new Quaternion(turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
                turretBackLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);

                turretFrontLegL2.localRotation = Powers_AnimMath.Slide(turretFrontLegL2.localRotation, Quaternion.Euler(15, 15, 0), 0.01f);
                turretFrontLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
                turretBackLegL2.localRotation = new Quaternion(turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
                turretBackLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
            }
        }
        else if (turretAI.agent.velocity.magnitude > 0.1f)
        {
            //These are used to animate the legs while walking.
            turretFrontLegL1.localRotation = Powers_AnimMath.Slide(turretFrontLegL1.localRotation, Quaternion.Euler((turretWalkWave2 * -2.5f) - 2.5f, 0, (turretWalkWave1 * -45) - 45), 0.01f);
            turretFrontLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
            turretBackLegL1.localRotation = new Quaternion(turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
            turretBackLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);

            turretFrontLegL2.localRotation = Powers_AnimMath.Slide(turretFrontLegL2.localRotation, Quaternion.Euler(10 - turretWalkWave2 * 5, 10 - turretWalkWave2 * 5, 0), 0.01f);
            turretFrontLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
            turretBackLegL2.localRotation = new Quaternion(turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
            turretBackLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
        }

    }

    private void LookAtPlayer()
    {
        //Rotate the turret top to face the player.
        Vector3 dir = transform.position - turretAI.playerPos.position;
        //Prevent rotation up and down
        dir.y = 0;

        //set the target rotation to the proper value
        targetRot = Quaternion.LookRotation(dir);
        dir = targetRot.eulerAngles;
        dir = new Vector3(0, 0, targetRot.eulerAngles.y-transform.eulerAngles.y+180);
        targetRot = Quaternion.Euler(dir);

        //Rotate the turret top
        turretTop.localRotation = Powers_AnimMath.Slide(turretTop.localRotation, targetRot, 0.01f);
    }

    private void Death()
    {
        //Turret AI immediately gets deactivated to prevent the turret from moving or shooting.
        turretAI.agent.enabled = false;
        turretAI.enabled = false;

        //Change and rotate eye to correct position.
        turretEye.material = deadEyeMat;
        turretEye.transform.localRotation = Powers_AnimMath.Slide(turretEye.transform.localRotation, Quaternion.Euler(0, 90, 45), 0.001f);
        turretEye.transform.localScale = Powers_AnimMath.Slide(turretEye.transform.localScale, new Vector3(0.2f, 0.2f, 0.2f), 0.001f);

        //These animate the turret's body.
        turretBody.localPosition = Powers_AnimMath.Slide(turretBody.localPosition, new Vector3(0, 0.3f, 0), 0.001f);

        turretFrontLegL1.localRotation = Powers_AnimMath.Slide(turretFrontLegL1.localRotation, Quaternion.Euler(0, 0, 0), 0.001f);
        turretFrontLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
        turretBackLegL1.localRotation = new Quaternion(turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, -turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);
        turretBackLegR1.localRotation = new Quaternion(-turretFrontLegL1.localRotation.x, -turretFrontLegL1.localRotation.y, turretFrontLegL1.localRotation.z, turretFrontLegL1.localRotation.w);

        turretFrontLegL2.localRotation = Powers_AnimMath.Slide(turretFrontLegL2.localRotation, Quaternion.Euler(60, 60, 0), 0.001f);
        turretFrontLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
        turretBackLegL2.localRotation = new Quaternion(turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);
        turretBackLegR2.localRotation = new Quaternion(-turretFrontLegL2.localRotation.x, -turretFrontLegL2.localRotation.y, turretFrontLegL2.localRotation.z, turretFrontLegL2.localRotation.w);

        turretWingL.localRotation = Powers_AnimMath.Slide(turretWingL.localRotation, Quaternion.Euler(20, 0, 0), 0.001f);
        turretWingR.localRotation = Powers_AnimMath.Slide(turretWingR.localRotation, Quaternion.Euler(30, 0, 0), 0.001f);
    }
}
