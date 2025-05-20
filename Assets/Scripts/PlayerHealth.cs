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

    void Start()
    {
        diePanel.SetActive(false);
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return; // dead

        currentHealth -= damageAmount;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);
        
        if (currentHealth <= 0)
        {
            currentHealth = 0; // Health stay at 0
            Die();
        }
    }
    
    void Die()
    {
        Instantiate(bloodPrefab, transform.position, Quaternion.identity);
        diePanel.SetActive(true);
        playerMovement.SetMovementEnabled(false);
        StartCoroutine(GoToGameOverScene());
    }

    private IEnumerator GoToGameOverScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(6);
    }
    
}
