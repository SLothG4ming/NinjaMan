using UnityEngine;

public class NinjaRangeDetection : MonoBehaviour
{
    private NinjaController ninjaController;

    private void Start()
    {
        // Find the NinjaController script on the parent object
        ninjaController = GetComponentInParent<NinjaController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ninjaController != null)
        {
            // Check if the player is in front of the Ninja
            bool isPlayerInFront = ninjaController.IsPlayerInFront(other.transform.position);

            if (isPlayerInFront)
            {
                Debug.Log("Player is in range and in front of the Ninja.");
                // Start attacking (play attack animation, deal damage, etc.)
                ninjaController.StartAttack();
            }
            else
            {
                Debug.Log("Player is in range but not in front of the Ninja.");
            }
        }
    }

}
