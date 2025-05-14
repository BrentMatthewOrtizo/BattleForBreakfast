using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    // Renamed 'animator' to 'playerCharacterAnimator' for clarity
    private Animator playerCharacterAnimator;
    private PlayerMovement playerMovement; // To interact with movement state

    // --- NEW: Reference for the Sword Slash Effect's Animator ---
    [Header("Effect Configuration")]
    [Tooltip("Assign the Animator component from the SlashEffectGFX child GameObject here.")]
    public Animator swordEffectAnimator; // The Animator on SlashEffectGFX

    // Flag managed by Animation Events on PLAYER'S CharSlash animations
    // Renamed 'isCurrentlyAttacking' to 'isCharacterCurrentlyAttacking' for clarity
    private bool isCharacterCurrentlyAttacking = false;

    void Awake()
    {
        // Get the Animator for the player character itself
        playerCharacterAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        if (playerCharacterAnimator == null) Debug.LogError("PlayerAttack: PlayerCharacterAnimator (Player's own Animator) not found!");
        if (playerMovement == null) Debug.LogError("PlayerAttack: PlayerMovement component not found!");
        // --- NEW: Check if the swordEffectAnimator is assigned ---
        if (swordEffectAnimator == null) Debug.LogError("PlayerAttack: SwordEffectAnimator (for slash visual) has NOT been assigned in the Inspector!");
    }

    // Called by the Player Input component when the "Attack" action is performed
    public void OnAttack(InputAction.CallbackContext context)
    {
        // Attack only if button pressed, not already attacking, and components are valid
        if (context.performed && !isCharacterCurrentlyAttacking &&
            playerCharacterAnimator != null && playerMovement != null) // swordEffectAnimator null check will happen in TriggerSwordEffectVisual
        {
            playerMovement.UpdateAimDirectionTowardsMouse(); // Ensure player faces mouse
            playerCharacterAnimator.SetTrigger("isAttacking"); // Trigger player's CharSlash animation
                                                              // The sword effect itself will be triggered by an Animation Event
        }
    }

    // --- Animation Event Methods for PLAYER'S CharSlash animations ---

    // Called from START Animation Event on each of Player's CharSlash animations
    // Renamed 'AttackAnimationStarted' to 'CharacterAttackAnimStarted'
    public void CharacterAttackAnimStarted()
    {
        isCharacterCurrentlyAttacking = true;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false); // Restrict player movement during attack
        }
    }

    // --- NEW METHOD: Called from an Animation Event on Player's CharSlash animations AT THE POINT OF IMPACT ---
    public void TriggerSwordEffectVisual()
    {
        if (swordEffectAnimator == null || playerCharacterAnimator == null)
        {
            if (swordEffectAnimator == null) Debug.LogError("PlayerAttack: SwordEffectAnimator is null when trying to TriggerSwordEffectVisual. Assign it in Inspector.");
            if (playerCharacterAnimator == null) Debug.LogError("PlayerAttack: PlayerCharacterAnimator is null when trying to TriggerSwordEffectVisual.");
            return;
        }

        // Get the direction player is facing (which was set by UpdateAimDirectionTowardsMouse in OnAttack)
        float aimX = playerCharacterAnimator.GetFloat("LastInputX");
        float aimY = playerCharacterAnimator.GetFloat("LastInputY");

        swordEffectAnimator.SetFloat("LastInputX", aimX);
        swordEffectAnimator.SetFloat("LastInputY", aimY);
        swordEffectAnimator.SetTrigger("PlaySlash"); // "PlaySlash" must match trigger name in SlashEffectAnimatorController
        // Debug.Log($"Sword Effect Triggered: AimX={aimX}, AimY={aimY}");
    }

    // Called from END Animation Event on each of Player's CharSlash animations
    // Renamed 'AttackAnimationFinished' to 'CharacterAttackAnimFinished'
    public void CharacterAttackAnimFinished()
    {
        isCharacterCurrentlyAttacking = false;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true); // Re-enable player movement
        }
    }
}