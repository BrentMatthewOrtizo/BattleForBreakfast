using UnityEngine;

public class EnemyAttackDamage : MonoBehaviour
{
    [Tooltip("How much damage this specific enemy attack/hitbox deals.")]
    public int damageAmount = 5; // Default, you can change this per enemy hitbox in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Log every collision this trigger detects for debugging
        // Debug.Log(transform.root.name + "'s hitbox (" + gameObject.name + ") collided with: " + other.gameObject.name + " with tag: " + other.tag);

        // Check if the object we collided with is tagged "Player"
        if (other.CompareTag("Player"))
        {
            // Try to get the IDamageable component from the Player object
            IDamageable playerDamageable = other.GetComponent<IDamageable>();

            if (playerDamageable != null)
            {
                // If the Player has an IDamageable component, tell it to take damage
                playerDamageable.TakeDamage(damageAmount);
                Debug.Log(transform.root.name + " (enemy) dealt " + damageAmount + " damage to " + other.name + " (Player)");

                // Optional: Deactivate this specific hitbox after it hits the player once per activation.
                // This prevents a single enemy swing from hitting the player multiple times if they stay overlapped.
                // The hitbox will be re-enabled by EnemyMovement.cs for the next attack.
                // gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("EnemyAttackDamage: " + other.name + " (Player) was hit but has no IDamageable component (e.g., PlayerHealth script not attached or not implementing IDamageable).");
            }
        }
    }
}