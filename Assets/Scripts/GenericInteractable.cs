using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GenericInteractable : MonoBehaviour, IInteractable
{
    public enum Mode { Nothing, LoadScene, Teleport }
    public Mode mode = Mode.Nothing;
    
    public int sceneBuildIndex;
    public string spawnPointID;
    
    public Transform teleportDestination;
    public PolygonCollider2D newMapBoundary;
    
    Transform _player;
    CinemachineConfiner2D _confiner;
    bool _didCacheTeleportRefs = false;
    
    void CacheTeleportRefsIfNeeded()
    {
        if (_didCacheTeleportRefs || mode != Mode.Teleport)
            return;

        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            _player = playerGO.transform;

        _confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        _didCacheTeleportRefs = true;
    }

    public bool CanInteract() => true;

    public void Interact()
    {
        switch (mode)
        {
            case Mode.LoadScene:
                if (!string.IsNullOrEmpty(spawnPointID))
                    SpawnManager.nextSpawnPointID = spawnPointID;
                SceneManager.LoadScene(sceneBuildIndex);
                break;

            case Mode.Teleport:
                CacheTeleportRefsIfNeeded();

                if (_player == null)
                {
                    return;
                }

                if (newMapBoundary != null && _confiner != null)
                    _confiner.BoundingShape2D = newMapBoundary;

                if (teleportDestination != null)
                    _player.position = teleportDestination.position;
                break;

            case Mode.Nothing:
            default:
                break;
        }
    }
}