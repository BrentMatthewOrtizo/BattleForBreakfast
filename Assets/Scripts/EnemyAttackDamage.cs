using UnityEngine;

public class EnemyAttackDamage : MonoBehaviour
{
    [Tooltip("How much damage this specific enemy attack/hitbox deals.")]
    public int damageAmount = 5;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[EnemyAttackDamage] Hitbox '{gameObject.name}' on '{transform.root.name}' triggered with '{other.gameObject.name}' (Tag: '{other.tag}')");

        // Check if the object we collided with is tagged "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[EnemyAttackDamage] '{gameObject.name}' hit something tagged 'Player': '{other.gameObject.name}'");
            
            IDamageable playerDamageable = other.GetComponent<IDamageable>();
            
            if (playerDamageable != null)
            {
                Debug.Log($"[EnemyAttackDamage] Found IDamageable component on '{other.gameObject.name}'. Attempting to deal {damageAmount} damage.");
                playerDamageable.TakeDamage(damageAmount);
                Debug.Log($"[EnemyAttackDamage] {transform.root.name} (enemy) successfully dealt {damageAmount} damage to {other.name} (Player). Player should take damage now.");
                
            }
            else
            {
                Debug.LogWarning($"[EnemyAttackDamage] Hit '{other.name}' (Player) but it has NO IDamageable component. Check if PlayerHealth.cs is on '{other.gameObject.name}' and implements IDamageable.");
            }
        }
    }
    
    void OnEnable()
    {
        Debug.Log($"[EnemyAttackDamage] Hitbox '{gameObject.name}' on enemy '{transform.root.name}' has been ENABLED.");
    }

    void OnDisable()
    {
        Debug.Log($"[EnemyAttackDamage] Hitbox '{gameObject.name}' on enemy '{transform.root.name}' has been DISABLED.");
    }
}