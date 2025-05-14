using UnityEngine;
using System.Collections.Generic;

public class SpawnZone : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public int maxEnemies = 15;
    public float spawnInterval = 2f;

    private float spawnTimer;
    private bool playerInside = false;

    private PolygonCollider2D area;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Transform player;

    void Awake()
    {
        area = GetComponent<PolygonCollider2D>();
        area.isTrigger = true; // Set this in Inspector too
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        // Clean dead enemies
        activeEnemies.RemoveAll(e => e == null);

        if (playerInside && activeEnemies.Count < maxEnemies)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector2 spawnPos = GetRandomPointInZone();
        GameObject newEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    private Vector2 GetRandomPointInZone()
    {
        Bounds bounds = area.bounds;
        Vector2 point;
        int attempts = 0;
        do
        {
            point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
            attempts++;
        } while (!area.OverlapPoint(point) && attempts < 20);
        return point;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            spawnTimer = 0.5f; // optional: delay before first spawn
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