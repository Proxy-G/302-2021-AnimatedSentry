using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Powers_DeathUI : MonoBehaviour
{
    public Powers_HealthSystem playerHealth;
    public CanvasGroup healthbarHolder;
    public CanvasGroup deathUI;

    void Start()
    {
        healthbarHolder.alpha = 1;
        deathUI.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealth.health == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            healthbarHolder.alpha = Powers_AnimMath.Slide(healthbarHolder.alpha, 0, 0.02f);
            deathUI.alpha = Powers_AnimMath.Slide(deathUI.alpha, 1, 0.02f);
        }
    }

    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
