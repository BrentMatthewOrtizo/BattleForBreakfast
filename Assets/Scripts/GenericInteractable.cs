using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GenericInteractable : MonoBehaviour, IInteractable
{
    public enum Mode { Nothing, LoadScene, Teleport }
    [Header("What this does when you press E:")]
    public Mode mode = Mode.Nothing;

    [Header("→ Scene Loading (LoadScene only)")]
    public int sceneBuildIndex;

    [Header("→ Teleporting (Teleport only)")]
    public Transform teleportDestination;
    public PolygonCollider2D newMapBoundary;

    // cached at runtime, but only if we actually teleport
    Transform              _player;
    CinemachineConfiner2D _confiner;
    bool                  _didCacheTeleportRefs = false;
    
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
                SceneManager.LoadScene(sceneBuildIndex);
                break;

            case Mode.Teleport:
                CacheTeleportRefsIfNeeded();

                if (_player == null)
                {
                    Debug.LogError($"[{name}] cannot teleport: no Player found.");
                    return;
                }

                if (newMapBoundary != null && _confiner != null)
                    _confiner.BoundingShape2D = newMapBoundary;

                if (teleportDestination != null)
                    _player.position = teleportDestination.position;
                else
                    Debug.LogWarning($"[{name}] has mode=Teleport but no destination set.");
                break;

            case Mode.Nothing:
            default:
                Debug.Log($"[{name}] does nothing on interact.");
                break;
        }
    }
}