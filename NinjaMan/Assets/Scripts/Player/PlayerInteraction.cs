
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{

    [SerializeField] private Transform _interactitionTransform;
    [SerializeField] private float _interactionDistance = 1.5f;

    

    
    public void OnInteraction(InputAction.CallbackContext context)

    {
        if (context.performed)
        {
            Debug.Log("Hit  Somethin");
            RaycastHit2D hit = Physics2D.Raycast(_interactitionTransform.transform.position, _interactitionTransform.transform.right, _interactionDistance);

            if (hit.collider == null)
            {
                return;
            }
            else
            {
                if (hit.transform.TryGetComponent(out IInteractable interactableObject))
                {
                    interactableObject.StartInteraction();
                }

            }
        }


    }



}
