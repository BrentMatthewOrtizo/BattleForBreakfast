using Unity.Cinemachine;
using UnityEngine;

public class PresentToEnd : MonoBehaviour, IInteractable
{

    public GameObject player;
    public PolygonCollider2D mapBoundary;
    public CinemachineConfiner2D confiner;
    
    private Vector2 teleportLocation = new Vector2(56f, -35f);

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