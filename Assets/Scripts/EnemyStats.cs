using UnityEngine;

    [CreateAssetMenu(fileName = "NewEnemyStats", menuName = "MyGame/Enemy Stats")]
    public class EnemyStats : ScriptableObject
    {
        [Header("Core References & Identity")]
        public GameObject enemyPrefab; // Assign your actual enemy prefab here
        public string enemyName = "Default Enemy Name";

        [Header("Combat Stats")]
        public float health = 100f;
        public float moveSpeed = 3f;
        public float attackDamage = 10f;
        public float attackRange = 1.5f; // Example
        public float attackCooldown = 2f; // Example

        [Header("Detection & Behavior")]
        public float detectionRadius = 8f; // Example

        // Add any other stats your enemies might need (e.g., armor, scoreValue, specific sounds)
    }
