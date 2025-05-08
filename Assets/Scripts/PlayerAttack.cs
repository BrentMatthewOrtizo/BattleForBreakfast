using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement; // Reference to PlayerMovement script

    // This flag is managed by Animation Events on your "CharSlash" animations
    private bool isCurrentlyAttacking = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        if (animator == null)
        {
            Debug.LogError("PlayerAttack: Animator component not found on this GameObject!");
        }
        if (playerMovement == null)
        {
            Debug.LogError("PlayerAttack: PlayerMovement component not found on this GameObject!");
        }
    }

    // Called by the Player Input component for the "Attack" action
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isCurrentlyAttacking && animator != null && playerMovement != null)
        {
            // Update aim direction based on mouse just before triggering attack.
            playerMovement.UpdateAimDirectionTowardsMouse();

            // Trigger the attack animation in the Animator
            animator.SetTrigger("isAttacking"); // "isAttacking" must match your Animator trigger parameter
        }
    }

    // --- Animation Event Methods ---
    // Called by events placed on your "CharSlash" (or equivalent) animation clips.

    // Call this from an Animation Event at the START of ALL your attack animations
    public void AttackAnimationStarted()
    {
        isCurrentlyAttacking = true;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false); // Disable player movement during the attack
        }
    }

    // Call this from an Animation Event at the END of ALL your attack animations
    public void AttackAnimationFinished()
    {
        isCurrentlyAttacking = false;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true); // Re-enable player movement after the attack
        }
    }
}