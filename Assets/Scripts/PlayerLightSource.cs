using UnityEngine;

public class PlayerLightSource : MonoBehaviour
{
    [Tooltip("The Material used for the darkness overlay (should use LightMaskShader). Assign this in the Inspector.")]
    public Material lightMaskMaterial;

    [Tooltip("The Transform of the player GameObject. Assign this in the Inspector, or it will try to find by tag 'Player'.")]
    public Transform playerTransform;

    [Tooltip("The radius of the light around the player.")]
    public float lightRadius = 7f;

    [Tooltip("The base color of the darkness (usually black). Alpha will be controlled by the shader.")]
    public Color darknessColor = Color.black; // Make sure alpha is 1 (or 255 in color picker) for opaque darkness

    void Start()
    {
        if (lightMaskMaterial == null)
        {
            Debug.LogError("PlayerLightSource: Light Mask Material is not assigned! Please assign it in the Inspector.", this);
            enabled = false; // Disable script if material is missing
            return;
        }

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                Debug.Log("PlayerLightSource: Player Transform automatically found using tag 'Player'.", this);
            }
            else
            {
                // This error means it wasn't assigned AND couldn't be found by tag
                Debug.LogError("PlayerLightSource: Player Transform is not assigned and could not be found by tag 'Player'! Please assign it or tag your player.", this);
                enabled = false; // Script disables itself
                return;
            }
        }

        // Ensure the material properties are set initially when the game starts
        UpdateShaderProperties();
    }

    void Update()
    {
        Debug.Log("PlayerLightSource Update is running. Player Transform: " + (playerTransform != null ? playerTransform.name : "NULL")); // ADD THIS
        if (playerTransform != null && lightMaskMaterial != null)
        {
            UpdateShaderProperties();
        }
    }

    void UpdateShaderProperties()
    {
        // --- Add this line ---
        Debug.Log($"Updating shader. Player Pos: {playerTransform.position}, Radius: {lightRadius}");

        lightMaskMaterial.SetVector("_PlayerWorldPos", playerTransform.position);
        lightMaskMaterial.SetFloat("_LightRadius", lightRadius);
        lightMaskMaterial.SetColor("_DarknessColor", darknessColor);
    }

    // This function is called in the editor when the script is loaded or a value is changed in the Inspector.
    // It's useful for seeing changes in Edit Mode if you had a way to preview, but mostly for runtime.
    void OnValidate()
    {
        // If the game is playing and references are set, update the shader.
        // This helps if you tweak values in the Inspector during play mode.
        if (Application.isPlaying && lightMaskMaterial != null && playerTransform != null)
        {
            UpdateShaderProperties();
        }
        // Ensure darknessColor has full alpha by default if not set via inspector correctly
        if (darknessColor.a < 1.0f)
        {
            // darknessColor.a = 1.0f; // Uncomment if you want to force full alpha on the base color
        }
    }
}