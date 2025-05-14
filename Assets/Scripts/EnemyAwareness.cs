using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{

    public bool awareOfPlayer { get; private set; }
    public Vector2 directionToPlayer { get; private set; }
    public float awarenessDistance = 6.5f;

    private Transform playerTransform;
    
    void Awake()
    {
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
    }
    
    void Update()
    {
        Vector2 enemyToPlayerVector = playerTransform.position - transform.position;
        directionToPlayer = enemyToPlayerVector.normalized;
        
        if (enemyToPlayerVector.magnitude <= awarenessDistance)
            awareOfPlayer = true;
        else
            awareOfPlayer = false;
    }
}
