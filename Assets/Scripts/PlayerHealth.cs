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

    // public GameObject gameOverScreen; later for game over screen

    void Start()
    {
        diePanel.SetActive(false);
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    
    // HP add-on for testing
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
            currentHealth = 0; // Clamp health at 0
            Die();
        }
    }
    
    void Die()
    {
        //TO DO 
        diePanel.SetActive(true);
        Debug.Log(gameObject.name + " has died!");
        playerMovement.SetMovementEnabled(false);
        StartCoroutine(GoToGameOverScene());
    }

    private IEnumerator GoToGameOverScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(6);
    }
    
}
