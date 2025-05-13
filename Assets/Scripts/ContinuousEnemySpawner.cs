using UnityEngine;
using System.Collections.Generic; // For List

public class ContinuousEnemySpawner : MonoBehaviour
{
    [Header("Spawning Configuration")]
    public EnemyStats[] enemyTypesToSpawn; // Array of different enemy stats (and thus prefabs) to spawn
    public int maxEnemies = 15;
    public float spawnInterval = 10f; // Time in seconds between spawn attempts

    [Header("Spawn Area")]
    public Vector2 spawnAreaCenter = Vector2.zero;
    public Vector2 spawnAreaSize = new Vector2(50f, 50f); // The dimensions of the rectangular spawn area
    public LayerMask obstacleLayerMask; // Layer(s) that would block spawning (e.g., walls, props)
    public float minSpawnCheckRadius = 0.5f; // Radius to check if spawn spot is clear

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float currentSpawnTimer;

    void Start()
    {
        if (enemyTypesToSpawn == null || enemyTypesToSpawn.Length == 0)
        {
            Debug.LogError("No enemy types assigned to the spawner! Disabling spawner.", this);
            this.enabled = false;
            return;
        }
        foreach (var stats in enemyTypesToSpawn)
        {
            if (stats.enemyPrefab == null)
            {
                Debug.LogError("EnemyStats asset '" + stats.name + "' is missing its enemyPrefab reference! This enemy type will not spawn.", this);
            }
        }

        currentSpawnTimer = spawnInterval;
    }

    void Update()
    {
        
        activeEnemies.RemoveAll(enemy => enemy == null);

        currentSpawnTimer -= Time.deltaTime;

        if (currentSpawnTimer <= 0f)
        {
            TrySpawnEnemy();
            currentSpawnTimer = spawnInterval; // Reset timer
        }
    }

    void TrySpawnEnemy()
    {
        if (activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // 1. Select an enemy type to spawn (randomly from the list of valid types)
        List<EnemyStats> validEnemyTypes = new List<EnemyStats>();
        foreach (var stats in enemyTypesToSpawn)
        {
            if (stats.enemyPrefab != null) // Only consider types that have a prefab assigned
            {
                validEnemyTypes.Add(stats);
            }
        }

        if (validEnemyTypes.Count == 0)
        {
            return;
        }
        EnemyStats selectedStats = validEnemyTypes[Random.Range(0, validEnemyTypes.Count)];


        // 2. Determine a random spawn position
        Vector2 spawnPosition = Vector2.zero; // Initialize
        bool positionFound = false;
        int attempts = 0; // To prevent infinite loop if area is too crowded or small

        while (!positionFound && attempts < 20) // Try a reasonable number of times to find a clear spot
        {
            float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
            float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
            spawnPosition = new Vector2(randomX, randomY);

            // 3. Check if the spawn position is clear (not inside an obstacle)
            Collider2D hit = Physics2D.OverlapCircle(spawnPosition, minSpawnCheckRadius, obstacleLayerMask);
            if (hit == null) // No obstacle hit
            {
                positionFound = true;
            }
            attempts++;
        }

        if (!positionFound)
        {
            return;
        }

        // 4. Instantiate the enemy
        GameObject newEnemyGo = Instantiate(selectedStats.enemyPrefab, spawnPosition, Quaternion.identity);
        newEnemyGo.name = selectedStats.enemyName + "_Spawner_" + Time.frameCount; // Unique name for debugging

        // 5. Configure the spawned enemy
        EnemyAI enemyAI = newEnemyGo.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.enemyStats = selectedStats; // Assign the ScriptableObject stats
            enemyAI.spawner = this;             // Give it a reference back to this spawner
        }
        else
        {
            Debug.LogError("Spawned enemy prefab '" + selectedStats.enemyPrefab.name + "' is missing an EnemyAI component! Destroying instance.", newEnemyGo);
            Destroy(newEnemyGo); // Destroy invalid enemy before adding to list
            return;
        }

        activeEnemies.Add(newEnemyGo);
    }

    // Called by EnemyAI when an enemy is defeated
    public void OnEnemyDefeated(GameObject enemyInstance)
    {
        // It's good practice to check if it's actually in the list before removing
        if (activeEnemies.Contains(enemyInstance))
        {
            activeEnemies.Remove(enemyInstance);
        }
    }

    // Visualize the spawn area in the editor for easier setup
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Green, semi-transparent
        Vector3 cubeCenter = new Vector3(spawnAreaCenter.x, spawnAreaCenter.y, 0);
        Vector3 cubeSize = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f); // Small depth for visibility
        Gizmos.DrawCube(cubeCenter, cubeSize);

        Gizmos.color = Color.green; // Solid green outline
        Gizmos.DrawWireCube(cubeCenter, cubeSize);
    }
}