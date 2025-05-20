using System.Collections;
using UnityEngine;

public class EnemyAttackDamage : MonoBehaviour
{
    
    public int damageAmount = 5;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable playerDamageable = other.GetComponent<IDamageable>();
            
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(damageAmount);
            }
        }

    }
    
}