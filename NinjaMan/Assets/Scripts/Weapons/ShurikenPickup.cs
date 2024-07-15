using UnityEngine;

public class ShurikenPickup : MonoBehaviour
{
    [SerializeField] private int shurikansToAdd = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Add shurikens to the player's count
                playerController.AddShurikans(shurikansToAdd);

                // Update the shuriken UI directly from PlayerController
                playerController.UpdateShurikenUI();

                // Destroy the pickup
                Destroy(gameObject);
            }
        }
    }
}
