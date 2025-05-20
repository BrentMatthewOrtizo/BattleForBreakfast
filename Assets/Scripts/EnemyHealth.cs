using System.Collections;
using UnityEngine;


public class EnemyHealth : MonoBehaviour, IDamageable
{
   [Tooltip("Starting and maximum health for this enemy type.")]
   public int maxHealth = 10;
   public int currentHealth;
   public GameObject bloodPrefab;
  
   private SpriteRenderer spriteRenderer;
   public Material flashMaterial;
   private Material originalMaterial;
   public float flashDuration = 0.1f;
   private Coroutine flashCoroutine;
  


   void Awake()
   {
       spriteRenderer = GetComponent<SpriteRenderer>();
       if (spriteRenderer == null)
       {
           spriteRenderer = GetComponentInChildren<SpriteRenderer>();
       }


       if (spriteRenderer != null)
       {
           originalMaterial = spriteRenderer.material;
       }
       else
       {
           Debug.LogError("EnemyHealth: SpriteRenderer component not found on " + gameObject.name + " or its children.", this);
       }


       currentHealth = maxHealth;
   }


   public void TakeDamage(int damageAmount)
   {
       if (currentHealth <= 0) return;


       currentHealth -= damageAmount;
       Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);
      
       if (spriteRenderer != null && flashMaterial != null && originalMaterial != null)
       {
           if (flashCoroutine != null)
           {
               StopCoroutine(flashCoroutine);
               spriteRenderer.material = originalMaterial; // Ensure it's reset if interrupted
           }
           flashCoroutine = StartCoroutine(FlashEffect());
       }
       else if (flashMaterial == null)
       {
           Debug.LogWarning("EnemyHealth: FlashMaterial not assigned for " + gameObject.name, this);
       }


       if (currentHealth <= 0)
       {
           currentHealth = 0;
           Die();
       }
   }


   private IEnumerator FlashEffect()
   {
       spriteRenderer.material = flashMaterial;
       yield return new WaitForSeconds(flashDuration);
       // Check if spriteRenderer still exists (e.g. object wasn't destroyed during flash)
       if (spriteRenderer != null)
       {
           spriteRenderer.material = originalMaterial; // Revert to original material
       }
       flashCoroutine = null;
   }


   void Die()
   {
       Instantiate(bloodPrefab, transform.position, Quaternion.identity);
       Debug.Log(gameObject.name + " has died!", this);
       Destroy(gameObject);
   }
}