using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable 
{
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

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Clamp health at 0
            Die();
        }
    }

    void Die()
    {
        Instantiate(bloodPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}