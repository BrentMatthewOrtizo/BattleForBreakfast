using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class SpawnManager : MonoBehaviour
{
    public static string nextSpawnPointID;
    public static bool   hasCereal = false;

    CinemachineConfiner2D _confiner;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(nextSpawnPointID))
            return;

        // find (or cache) your CinemachineConfiner2D on the active vcam
        if (_confiner == null)
            _confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        // pull all SpawnPoints in the scene
        var points = Object.FindObjectsByType<SpawnPoint>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var sp in points)
        {
            if (sp.spawnID == nextSpawnPointID)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    player.transform.position = sp.transform.position;
                
                if (_confiner != null && sp.cameraBoundary != null)
                    _confiner.BoundingShape2D = sp.cameraBoundary;

                break;
            }
        }

        nextSpawnPointID = null;
    }
}