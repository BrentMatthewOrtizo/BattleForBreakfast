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
            Debug.LogError("No light mask material assigned.", this);
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
        {
            UpdateShaderProperties();
        }
    }

    void UpdateShaderProperties()
    {

        lightMaskMaterial.SetVector("_PlayerWorldPos", playerTransform.position);
        lightMaskMaterial.SetFloat("_LightRadius", lightRadius);
        lightMaskMaterial.SetColor("_DarknessColor", darknessColor);
    }
}