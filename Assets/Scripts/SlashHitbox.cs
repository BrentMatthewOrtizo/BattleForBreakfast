using System.Collections;
using UnityEngine;

public class SlashHitbox : MonoBehaviour
{
    public static int damageAmount = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        
        if (damageableObject != null)
        {
            if (other.CompareTag("Player") && gameObject.transform.root.CompareTag("Player"))
            {
                return;
            }
            damageableObject.TakeDamage(damageAmount);
            Debug.Log(transform.root.name + " dealt " + damageAmount + " damage to " + other.name);
        }
    }
    
}