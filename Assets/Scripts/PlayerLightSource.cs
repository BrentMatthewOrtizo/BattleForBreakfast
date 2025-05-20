using UnityEngine;

public class PlayerLightSource : MonoBehaviour
{
    public Material lightMaskMaterial;
    public Transform playerTransform;
    public float lightRadius = 7f;
    public Color darknessColor = Color.black;

    void Start()
    {
        if (lightMaskMaterial == null)
        {
            Debug.LogError("PlayerLightSource: Light Mask Material is not assigned! Please assign it in the Inspector.", this);
            enabled = false;
            return;
        }

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogError("PlayerLightSource: Player Transform is not assigned and could not be found by tag 'Player'! Please assign it or tag your player.", this);
                enabled = false;
                return;
            }
        }
        UpdateShaderProperties();
    }

    void Update()
    {
        Debug.Log("PlayerLightSource Update is running. Player Transform: " + (playerTransform != null ? playerTransform.name : "NULL"));
        if (playerTransform != null && lightMaskMaterial != null)
        {
            UpdateShaderProperties();
        }
    }

    void UpdateShaderProperties()
    {
        Debug.Log($"Updating shader. Player Pos: {playerTransform.position}, Radius: {lightRadius}");

        lightMaskMaterial.SetVector("_PlayerWorldPos", playerTransform.position);
        lightMaskMaterial.SetFloat("_LightRadius", lightRadius);
        lightMaskMaterial.SetColor("_DarknessColor", darknessColor);
    }
    
    void OnValidate()
    {
        if (Application.isPlaying && lightMaskMaterial != null && playerTransform != null)
        {
            UpdateShaderProperties();
        }
        if (darknessColor.a < 1.0f)
        {
            // darknessColor.a = 1.0f;
        }
    }
}