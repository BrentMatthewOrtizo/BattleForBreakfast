using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    
    public GameObject interactionIcon;
    
    private IInteractable interactableInRange = null;
    
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Check if the action was performed AND if there is an interactable object in range
        if (context.performed && interactableInRange != null)
        {
            interactableInRange.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has an IInteractable component
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            // Further check if this interactable object CanInteract
            // This prevents trying to interact with an NPC that is already in dialogue, for example.
            if (interactable.CanInteract())
            {
                interactableInRange = interactable;
                interactionIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the exiting object is the one currently in range
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
    
}