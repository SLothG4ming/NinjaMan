using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Sprite[] healthStages;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetHealth(int health, int maxHealth)
    {
        slider.value = (float)health / maxHealth;

        int stageIndex = Mathf.Clamp(health, 0, healthStages.Length - 1);
        fillImage.sprite = healthStages[stageIndex];
    }
}