using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public GameObject slashHitbox_UR;
    public GameObject slashHitbox_UL;
    public GameObject slashHitbox_DL;
    public GameObject slashHitbox_DR;

    private Animator playerCharacterAnimator;
    private PlayerMovement playerMovement;

    [Header("Effect Configuration")]
    public Animator swordEffectAnimator;
    
    [Header("Audio")]
    public AudioClip sword_sfx; // Assign the sound effect in the Inspector
    private AudioSource audioSource; // AudioSource for playing the sound

    private bool isCharacterCurrentlyAttacking = false;

    void Awake()
    {
        playerCharacterAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // --- NEW: Add or get AudioSource component ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isCharacterCurrentlyAttacking &&
            playerCharacterAnimator != null && playerMovement != null)
        {
            playerMovement.UpdateAimDirectionTowardsMouse();
            playerCharacterAnimator.SetTrigger("isAttacking");
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

    public void CharacterAttackAnimStarted()
    {
        isCharacterCurrentlyAttacking = true;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }
    }

    public void TriggerSwordEffectVisual()
    {
        if (swordEffectAnimator == null || playerCharacterAnimator == null) return;

        float aimX = playerCharacterAnimator.GetFloat("LastInputX");
        float aimY = playerCharacterAnimator.GetFloat("LastInputY");

        swordEffectAnimator.SetFloat("LastInputX", aimX);
        swordEffectAnimator.SetFloat("LastInputY", aimY);
        swordEffectAnimator.SetTrigger("PlaySlash");

        // --- NEW: Play sword sound effect here ---
        if (sword_sfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(sword_sfx);
        }

        GameObject hitbox = GetHitboxByDirection(aimX, aimY);
        if (hitbox != null)
            StartCoroutine(ActivateHitbox(hitbox, 0.15f));
    }

    public void CharacterAttackAnimFinished()
    {
        isCharacterCurrentlyAttacking = false;
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }
    }

    private IEnumerator ActivateHitbox(GameObject hitbox, float duration)
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(duration);
        hitbox.SetActive(false);
    }
}
