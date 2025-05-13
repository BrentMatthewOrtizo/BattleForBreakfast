using UnityEngine;

    [CreateAssetMenu(fileName = "NewEnemyStats", menuName = "MyGame/Enemy Stats")]
    public class EnemyStats : ScriptableObject
    {
        
        [Header("Core References & Identity")]
        public GameObject enemyPrefab;
        public string enemyName = "Default Enemy Name";

        [Header("Combat Stats")]
        public float health = 100f;
        public float moveSpeed = 3.5f;
        public float attackDamage = 10f;
        public float attackRange = 1.5f;
        public float attackCooldown = 2f;

        [Header("Detection & Behavior")]
        public float detectionRadius = 8f;
        
    }
