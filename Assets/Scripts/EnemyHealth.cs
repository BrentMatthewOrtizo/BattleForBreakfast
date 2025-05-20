using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable 
{
    [Tooltip("Starting and maximum health for this enemy type.")]
    public int maxHealth = 10;
    public int currentHealth;
    public GameObject bloodPrefab;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return; // dead

        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Clamp health at 0
            Die();
        }
    }

    void Die()
    {
        Instantiate(bloodPrefab, transform.position, Quaternion.identity);
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }
}