using UnityEngine;
using UnityEngine.UI;

public class Cereal : MonoBehaviour, IInteractable
{
    
    public GameObject cerealSprite;
    public Image      cerealPanelImage;
    public GameObject doorInteraction;

    public void Start()
    {
        SetOpacity(0.2f);
        doorInteraction.SetActive(false);
    }

    public void Interact()
    {
        cerealSprite.SetActive(false);
        SpawnManager.hasCereal = true;
        SetOpacity(1f);
        
        doorInteraction.SetActive(true);
    }

    public bool CanInteract() => !SpawnManager.hasCereal;

    void SetOpacity(float a)
    {
        var c = cerealPanelImage.color;
        c.a = a;
        cerealPanelImage.color = c;
    }
    
}