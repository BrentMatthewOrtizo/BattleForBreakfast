using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("Unique ID used by SpawnManager to teleport here")]
    public string spawnID;

    [Tooltip("Which PolygonCollider2D the Cinemachine should confine to at this spawn")]
    public PolygonCollider2D cameraBoundary;
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        if (cameraBoundary != null)
            Gizmos.DrawWireCube(cameraBoundary.bounds.center, cameraBoundary.bounds.size);
    }
}