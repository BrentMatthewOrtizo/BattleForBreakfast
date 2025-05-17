using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable 
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    // public GameObject gameOverScreen; later for game over screen

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        // for testing of hp bar func
        if (Input.GetKeyDown(KeyCode.Space)) { TakeDamage(20); }
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
        Debug.Log(gameObject.name + " has died!");
        // - Play death animation
        // - Disable player controls (PlayerMovement, PlayerAttack)
        // - Show game over screen
    }
    
}
