using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image fillImage;
    [SerializeField] private Sprite[] healthStages;

    public void SetHealth(int health, int maxHealth)
    {
        float healthFraction = (float)health / maxHealth;
        fillImage.fillAmount = healthFraction;

        // Determine the stage index based on the health fraction
        int stageIndex = Mathf.Clamp(Mathf.FloorToInt(healthFraction * healthStages.Length), 0, healthStages.Length - 1);

        // Set the appropriate sprite based on the stage index
        if (stageIndex < healthStages.Length)
        {
            fillImage.sprite = healthStages[stageIndex];
        }

        // Update UI based on player's health
        playerController.MaxLife = maxHealth;
        playerController.CurrentLife = health;
    }
}
