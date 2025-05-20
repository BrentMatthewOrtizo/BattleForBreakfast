using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class SpawnZone : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] powerUpPrefabs;
    public int maxEnemies = 15;
    public float spawnInterval = 2f;
    
    public Tilemap collisionTilemap;
    public LayerMask noSpawnLayerMask;

    private float spawnTimer;
    private bool playerInside = false;

    private PolygonCollider2D area;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Transform player;

    void Awake()
    {
        area = GetComponent<PolygonCollider2D>();
        area.isTrigger = true;
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        activeEnemies.RemoveAll(e => e == null);

        if (playerInside && activeEnemies.Count < maxEnemies)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                SpawnPowerUp();
                spawnTimer = spawnInterval;
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector2 spawnPos = GetValidSpawnPosition();

        if (spawnPos != Vector2.zero)
        {
            GameObject newEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(newEnemy);
        }
    }

    private void SpawnPowerUp()
    {
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Vector2 spawnPos = GetValidSpawnPosition();

        if (spawnPos != Vector2.zero)
        {
            GameObject newPowerUp = Instantiate(prefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(newPowerUp);
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        Bounds bounds = area.bounds;
        Vector2 point;
        int attempts = 0;

        do
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            point = new Vector2(x, y);

            bool isInsideZone = area.OverlapPoint(point);
            bool overlapsBlockedTile = IsTileBlocked(point);
            bool overlapsPhysicsCollider = Physics2D.OverlapCircle(point, 0.4f, noSpawnLayerMask);

            if (isInsideZone && !overlapsBlockedTile && !overlapsPhysicsCollider)
                return point;

            attempts++;
        } while (attempts < 20);

        return Vector2.zero; // fallback
    }

    private bool IsTileBlocked(Vector2 worldPosition)
    {
        if (collisionTilemap == null)
            return false;

        Vector3Int cellPos = collisionTilemap.WorldToCell(worldPosition);
        return collisionTilemap.HasTile(cellPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            spawnTimer = 0.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}