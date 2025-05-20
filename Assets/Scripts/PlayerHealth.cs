using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public PlayerMovement playerMovement;
    public GameObject diePanel;
    public GameObject bloodPrefab;

    // --- Variables for shader flash effect ---
    private SpriteRenderer spriteRenderer;
    public Material flashMaterial; // Assign M_SpriteFlash in Inspector
    private Material originalMaterial;
    public float flashDuration = 0.1f;
    private Coroutine flashCoroutine;
    // --- End variables ---

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material; // Store the initial material
            // Log initial materials
            Debug.Log($"[PlayerHealth START] {gameObject.name} - Original Material Stored: {originalMaterial?.name} (Instance ID: {originalMaterial?.GetInstanceID()}) - Current SpriteRenderer Material is: {spriteRenderer.material?.name} (ID: {spriteRenderer.material?.GetInstanceID()})", this);
        }
        else
        {
            Debug.LogError($"[PlayerHealth START] {gameObject.name} - SpriteRenderer component not found!", this);
        }

        if (flashMaterial != null)
        {
            Debug.Log($"[PlayerHealth START] {gameObject.name} - Assigned Flash Material (from Inspector): {flashMaterial.name} (Instance ID: {flashMaterial.GetInstanceID()})", this);
        }
        else
        {
            Debug.LogError($"[PlayerHealth START] {gameObject.name} - FlashMaterial is NOT assigned in the Inspector!", this);
        }

        diePanel.SetActive(false);
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.LogWarning($"[PlayerHealth TakeDamage TOP] Player: {gameObject.name} - OriginalMat: {originalMaterial?.name} (ID: {originalMaterial?.GetInstanceID()}), FlashMat: {flashMaterial?.name} (ID: {flashMaterial?.GetInstanceID()}), CurrentMat: {spriteRenderer?.material?.name} (ID: {spriteRenderer?.material?.GetInstanceID()})", this);

        if (currentHealth <= 0 && diePanel.activeSelf)
        {
            Debug.Log($"[PlayerHealth TakeDamage] {gameObject.name} - Already dead and diePanel active. Ignoring further damage.", this);
            return;
        }

        Debug.Log($"[PlayerHealth TakeDamage] {gameObject.name} - Current Material BEFORE flash attempt (after initial checks): {spriteRenderer?.material?.name} (Instance ID: {spriteRenderer?.material?.GetInstanceID()})", this);

        currentHealth -= damageAmount;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        Debug.Log($"[PlayerHealth TakeDamage] {gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}", this);

        // --- Trigger flash effect ---
        if (spriteRenderer != null && flashMaterial != null && originalMaterial != null)
        {
            if (flashCoroutine != null)
            {
                Debug.Log($"[PlayerHealth TakeDamage] {gameObject.name} - Stopping existing flash coroutine.", this);
                StopCoroutine(flashCoroutine);
                // Force reset to original material immediately if interrupted
                spriteRenderer.material = originalMaterial;
                Debug.Log($"[PlayerHealth TakeDamage] {gameObject.name} - Material immediately reset to Original: {originalMaterial.name} (Instance ID: {originalMaterial.GetInstanceID()}). Current SpriteRenderer Material is: {spriteRenderer.material.name} (Instance ID: {spriteRenderer.material.GetInstanceID()})", this);
                flashCoroutine = null;
            }
            Debug.Log($"[PlayerHealth TakeDamage] {gameObject.name} - Starting new FlashEffect coroutine.", this);
            flashCoroutine = StartCoroutine(FlashEffect());
        }
        else
        {
            if(spriteRenderer == null) Debug.LogError($"[PlayerHealth TakeDamage] {gameObject.name} - SpriteRenderer is null. Cannot flash.", this);
            if(flashMaterial == null) Debug.LogError($"[PlayerHealth TakeDamage] {gameObject.name} - FlashMaterial is null. Assign it in Inspector. Cannot flash.", this);
            if(originalMaterial == null) Debug.LogError($"[PlayerHealth TakeDamage] {gameObject.name} - OriginalMaterial is null (was SpriteRenderer available in Start, or was its material null?). Cannot flash reliably.", this);
        }
        // --- End flash trigger ---

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (!diePanel.activeSelf)
            {
                Die();
            }
        }
    }

    private IEnumerator FlashEffect()
    {
        Debug.Log($"[PlayerHealth FlashEffect] {gameObject.name} - ENTERING Coroutine. Intended Original: {originalMaterial?.name} (ID: {originalMaterial?.GetInstanceID()}). Intended Flash: {flashMaterial?.name} (ID: {flashMaterial?.GetInstanceID()})", this);
        Debug.Log($"[PlayerHealth FlashEffect] {gameObject.name} - Current material BEFORE setting to flash: {spriteRenderer?.material?.name} (ID: {spriteRenderer?.material?.GetInstanceID()})", this);

        spriteRenderer.material = flashMaterial;
        Debug.Log($"[PlayerHealth FlashEffect] {gameObject.name} - Material SHOULD NOW BE FlashMaterial. Current SpriteRenderer Material is: {spriteRenderer.material.name} (Instance ID: {spriteRenderer.material.GetInstanceID()})", this);

        yield return new WaitForSeconds(flashDuration);

        Debug.Log($"[PlayerHealth FlashEffect] {gameObject.name} - Flash duration ({flashDuration}s) over. Attempting to revert material.", this);
        if (spriteRenderer != null)
        {
            Debug.Log($"[PlayerHealth FlashEffect] {gameObject.name} - Current material BEFORE revert attempt: {spriteRenderer.material?.name} (ID: {spriteRenderer.material?.GetInstanceID()})", this);
            // Check if we are still supposed to be using the flash material (sanity check)
            // This check can be problematic if originalMaterial IS flashMaterial, so we rely on proper storage in Start.
            // if (spriteRenderer.material == flashMaterial || spriteRenderer.material.name.Contains(flashMaterial.name)) // Comparing names can be a fallback if instance IDs are tricky due to instancing
            // {
            spriteRenderer.material = originalMaterial;
            Debug.Log($"[PlayerHealth FlashEffect] {gameObject.name} - Reverted material to OriginalMaterial: {originalMaterial.name} (Instance ID: {originalMaterial.GetInstanceID()}). Current SpriteRenderer Material IS NOW: {spriteRenderer.material.name} (Instance ID: {spriteRenderer.material.GetInstanceID()})", this);
            // }
            // else
            // {
            //     Debug.LogWarning($"[PlayerHealth FlashEffect] {gameObject.name} - Material was NOT flashMaterial ({flashMaterial.name}) before revert. It was: {spriteRenderer.material?.name}. Not reverting to avoid conflicts. Original should have been: {originalMaterial.name}", this);
            // }
        }
        else
        {
            Debug.LogWarning($"[PlayerHealth FlashEffect] {gameObject.name} - SpriteRenderer became null before material could be reverted (Player destroyed?).", this);
        }
        flashCoroutine = null;
        Debug.Log($"[PlayerHealth FlashEffect] {gameObject.name} - Coroutine FINISHED.", this);
    }

    void Die()
    {
        Debug.Log($"[PlayerHealth DIE] {gameObject.name} - Player is dying. Current material before Die logic: {spriteRenderer?.material?.name} (ID: {spriteRenderer?.material?.GetInstanceID()})", this);
        if (flashCoroutine != null)
        {
            Debug.Log($"[PlayerHealth DIE] {gameObject.name} - Active flash coroutine found. Stopping it.", this);
            StopCoroutine(flashCoroutine);
            if(spriteRenderer != null && originalMaterial != null)
            {
                spriteRenderer.material = originalMaterial;
                Debug.Log($"[PlayerHealth DIE] {gameObject.name} - Stopped flash coroutine and reset material to {originalMaterial.name} (ID: {originalMaterial.GetInstanceID()}) during Die sequence. Current material: {spriteRenderer.material.name} (ID: {spriteRenderer.material.GetInstanceID()})", this);
            }
            flashCoroutine = null;
        }

        Instantiate(bloodPrefab, transform.position, Quaternion.identity);
        diePanel.SetActive(true);
        Debug.Log($"{gameObject.name} has died!", this);
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }
        StartCoroutine(GoToGameOverScene());
    }

    private IEnumerator GoToGameOverScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(6);
    }
}