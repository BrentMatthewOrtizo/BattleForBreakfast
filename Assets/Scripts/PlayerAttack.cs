using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement; // To interact with movement state

    // Flag managed by Animation Events to prevent attack spamming / state issues
    private bool isCurrentlyAttacking = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        if (animator == null) Debug.LogError("PlayerAttack: Animator component not found!");
        if (playerMovement == null) Debug.LogError("PlayerAttack: PlayerMovement component not found!");
    }

    // Called by the Player Input component when the "Attack" action is performed
    public void OnAttack(InputAction.CallbackContext context)
    {
        // Attack only if button pressed, not already attacking, and components are valid
        if (context.performed && !isCurrentlyAttacking && animator != null && playerMovement != null)
        {
            playerMovement.UpdateAimDirectionTowardsMouse(); // Ensure attack aims at current mouse position
            animator.SetTrigger("isAttacking"); // Must match Trigger name in Animator Controller
        }
    }

    // --- Animation Event Methods ---
    // These are called by events placed on attack animation clips.

    // Called by an Animation Event at the START of attack animations
    public void AttackAnimationStarted()
    {
        isCurrentlyAttacking = true;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false); // Restrict player movement during attack
        }
    }

    // Called by an Animation Event at the END of attack animations
    public void AttackAnimationFinished()
    {
        isCurrentlyAttacking = false;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true); // Re-enable player movement
        }
    }
}