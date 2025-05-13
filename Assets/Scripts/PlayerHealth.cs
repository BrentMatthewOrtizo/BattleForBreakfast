using UnityEngine;
using UnityEngine.SceneManagement; // << REQUIRED for loading scenes

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (!gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("PlayerHealth script is on a GameObject that isn't tagged 'Player'. Enemies might not find it.", this);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log(gameObject.name + " took " + amount + " damage. Current health: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log(gameObject.name + " healed " + amount + ". Current health: " + currentHealth + "/" + maxHealth);
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log(gameObject.name + " has died! Loading scene with build index 5.");
        
        SceneManager.LoadScene(5);
    }
}