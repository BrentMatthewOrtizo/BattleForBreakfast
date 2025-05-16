using Unity.Cinemachine;
using UnityEngine;

public class StairToBasement : MonoBehaviour, IInteractable
{

    public GameObject player;
    public PolygonCollider2D mapBoundary;
    public CinemachineConfiner2D confiner;
    
    private Vector2 teleportLocation = new Vector2(55.55f, -6.07f);

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    public void Interact()
    {
        confiner.BoundingShape2D = mapBoundary;
        player.transform.position = teleportLocation;
    }

    public bool CanInteract()
    {
        return true;
    }
}
