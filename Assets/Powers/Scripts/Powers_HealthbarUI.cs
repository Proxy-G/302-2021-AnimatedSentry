using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Powers_HealthbarUI : MonoBehaviour
{
    public Powers_HealthSystem playerHealth;
    [Space(10)]
    public Image healthbarHolder;
    public Image healthbar;
    public Image healthbarHurt;

    private Vector3 healthbarHolderPos;

    void Start()
    {
        healthbarHolderPos = healthbarHolder.rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //Update health bar to player's health
        healthbar.rectTransform.sizeDelta = new Vector2((playerHealth.health / 100) * 250, 35);
        //Ease hurt health bar to represent damage taken
        healthbarHurt.rectTransform.sizeDelta = Powers_AnimMath.Slide(healthbarHurt.rectTransform.sizeDelta, new Vector2((playerHealth.health / 100) * 250, 35), 0.01f);
        //Shake healthbar based on how much damage the player has taken
        healthbarHolder.rectTransform.localPosition = new Vector3(healthbarHolderPos.x + (Random.Range(-25, 25)*(healthbar.rectTransform.sizeDelta.x/healthbarHurt.rectTransform.sizeDelta.x-1)), healthbarHolderPos.y + (Random.Range(-25, 25)*(healthbar.rectTransform.sizeDelta.x / healthbarHurt.rectTransform.sizeDelta.x-1)), healthbarHolderPos.z);
    }
}
