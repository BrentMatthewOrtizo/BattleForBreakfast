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
                enabled = false;
                return;
            }
        }
        UpdateShaderProperties();
    }

    void Update()
    {
        if (playerTransform != null && lightMaskMaterial != null)
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