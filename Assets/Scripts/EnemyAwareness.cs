using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{

    public bool awareOfPlayer { get; private set; }
    public Vector2 directionToPlayer { get; private set; }
    public float awarenessDistance = 6.5f;
    public LayerMask obstacleMask;
    public bool hasLineOfSight { get; private set; }

    private Transform playerTransform;
    
    void Awake()
    {
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
    }
    
    void Update()
    {
        Vector2 enemyToPlayerVector = playerTransform.position - transform.position;
        float distance = enemyToPlayerVector.magnitude;
        directionToPlayer = enemyToPlayerVector.normalized;

        awareOfPlayer = distance <= awarenessDistance;
        
        if (awareOfPlayer)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, awarenessDistance, obstacleMask);
            hasLineOfSight = hit.collider == null || hit.collider.CompareTag("Player");
        }
        else
        {
            hasLineOfSight = false;
        }
    }
}
