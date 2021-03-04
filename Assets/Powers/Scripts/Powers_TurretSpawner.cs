using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_TurretSpawner : MonoBehaviour
{
    public GameObject prefabTurret;
    public Powers_PlayerMovement playerScript;
    public float timeBetweenTurrets = 8;
    public int maxTurretsAllowed = 10;
    private float turretSpawnCountdown;

    private List<GameObject> turrets = new List<GameObject>();
    private float listCheckCountdown = 1;

    void Start()
    {
        turretSpawnCountdown = timeBetweenTurrets;
    }

    // Update is called once per frame
    void Update()
    {
        //countdown
        turretSpawnCountdown -= Time.deltaTime;
        listCheckCountdown -= Time.deltaTime;

        //Once timer complete, spawn turret.
        if (turretSpawnCountdown < 0 && turrets.Count < maxTurretsAllowed) SpawnTurret();

        //Once timer complete, check list for null turrets.
        if(listCheckCountdown < 0)
        {
            for (int i = turrets.Count - 1; i >= 0; i--)
            {
                if (turrets[i] == null) turrets.RemoveAt(i);
            }

            //Reset timer
            listCheckCountdown = 1;
        }
    }

    void SpawnTurret()
    {
        //random int to determine spawn location
        int location = Random.Range(0, 4);
        GameObject turret;

        //Spawn turret
        if (location == 0) turret = Instantiate(prefabTurret, new Vector3(18, 0, 18), Quaternion.identity);
        else if(location == 1) turret = Instantiate(prefabTurret, new Vector3(-18, 0, 10), Quaternion.identity);
        else if(location == 2) turret = Instantiate(prefabTurret, new Vector3(-18, 0, -18), Quaternion.identity);
        else turret = Instantiate(prefabTurret, new Vector3(18, 0, -18), Quaternion.identity);

        //set player script in turret prefab
        turret.GetComponent<Powers_TurretAI>().playerMovementScript = playerScript;

        //Add the spawned turret to the list of turrets.
        turrets.Add(turret);

        //reset timer
        turretSpawnCountdown = timeBetweenTurrets;
    }
}
