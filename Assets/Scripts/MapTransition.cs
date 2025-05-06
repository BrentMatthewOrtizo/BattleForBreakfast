using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    
    public PolygonCollider2D mapBoundary;
    public CinemachineConfiner2D confiner;
    public Direction direction;
    public float additivePos = 1f;

    public enum Direction {Up, Down, Left, Right}

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBoundary;
            UpdatePlayerPosition(collision.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += additivePos;
                break;
            case Direction.Down:
                newPos.y -= additivePos;
                break;
            case Direction.Left:
                newPos.x -= additivePos;
                break;
            case Direction.Right:
                newPos.x += additivePos;
                break;
        }
        
        player.transform.position = newPos;
    }
}
