using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour, IDamageable
{
   public static int maxHealth = 100;
   public static int currentHealth;
   public HealthBar healthBar;
   public PlayerMovement playerMovement;
   public GameObject diePanel;
   public GameObject bloodPrefab;
   
   private SpriteRenderer spriteRenderer;
   public Material flashMaterial;
   private Material originalMaterial;
   public float flashDuration = 0.1f;
   private Coroutine flashCoroutine;

   void Start()
   {
       spriteRenderer = GetComponent<SpriteRenderer>();
       if (spriteRenderer != null)
       {
           originalMaterial = spriteRenderer.material;
       }
       
       if (flashMaterial != null)
       {
           Debug.Log($"[PlayerHealth START] {gameObject.name} - Assigned Flash Material (from Inspector): {flashMaterial.name} (Instance ID: {flashMaterial.GetInstanceID()})", this);
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
       if (currentHealth <= 0 && diePanel.activeSelf)
       {
           return;
       }

       currentHealth -= damageAmount;
       if (healthBar != null)
       {
           healthBar.SetHealth(currentHealth);
       }

       if (spriteRenderer != null && flashMaterial != null && originalMaterial != null)
       {
           if (flashCoroutine != null)
           {
               StopCoroutine(flashCoroutine);
               spriteRenderer.material = originalMaterial;
               flashCoroutine = null;
           }
           flashCoroutine = StartCoroutine(FlashEffect());
       }

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
       spriteRenderer.material = flashMaterial;
       yield return new WaitForSeconds(flashDuration);
       
       if (spriteRenderer != null)
       {
           spriteRenderer.material = originalMaterial;
           // }
       }
       flashCoroutine = null;
   }


   void Die()
   {
       if (flashCoroutine != null)
       {
           StopCoroutine(flashCoroutine);
           if(spriteRenderer != null && originalMaterial != null)
           {
               spriteRenderer.material = originalMaterial;
           }
           flashCoroutine = null;
       }


       Instantiate(bloodPrefab, transform.position, Quaternion.identity);
       diePanel.SetActive(true);
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