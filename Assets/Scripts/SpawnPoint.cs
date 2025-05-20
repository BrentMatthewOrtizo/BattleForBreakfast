using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("Unique ID used by SpawnManager to teleport here")]
    public string spawnID;
    
    public PolygonCollider2D cameraBoundary;
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        if (cameraBoundary != null)
            Gizmos.DrawWireCube(cameraBoundary.bounds.center, cameraBoundary.bounds.size);
    }
}