using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    
    public GameObject slashHitbox_UR;
    public GameObject slashHitbox_UL;
    public GameObject slashHitbox_DL;
    public GameObject slashHitbox_DR;
    
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
    
    private GameObject GetHitboxByDirection(float x, float y)
    {
        if (x > 0 && y >= 0) return slashHitbox_UR;     // Right or UpRight
        if (x <= 0 && y > 0) return slashHitbox_UL;     // Up or UpLeft
        if (x < 0 && y <= 0) return slashHitbox_DL;     // Left or DownLeft
        if (x >= 0 && y < 0) return slashHitbox_DR;     // Down or DownRight
        return null;
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
        if (swordEffectAnimator == null || playerCharacterAnimator == null) return;

        float aimX = playerCharacterAnimator.GetFloat("LastInputX");
        float aimY = playerCharacterAnimator.GetFloat("LastInputY");

        swordEffectAnimator.SetFloat("LastInputX", aimX);
        swordEffectAnimator.SetFloat("LastInputY", aimY);
        swordEffectAnimator.SetTrigger("PlaySlash");

        GameObject hitbox = GetHitboxByDirection(aimX, aimY);
        if (hitbox != null)
            StartCoroutine(ActivateHitbox(hitbox, 0.15f)); // Keep it active for 0.15s
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
    
    private IEnumerator ActivateHitbox(GameObject hitbox, float duration)
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(duration);
        hitbox.SetActive(false);
    }
}