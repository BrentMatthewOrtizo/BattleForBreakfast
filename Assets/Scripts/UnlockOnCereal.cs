using UnityEngine;

public class UnlockOnCereal : MonoBehaviour, IInteractable
{
    
    private SpriteRenderer spriteRenderer;
    private bool hasRevealed = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetOpacity(0);
    }

    public void Interact()
    {
        hasRevealed = true;
        SetOpacity(1);
    }

    public bool CanInteract()
    {
        return SpawnManager.hasCereal && !hasRevealed;
    }

    private void SetOpacity(float opacity)
    {
        var color = spriteRenderer.color;
        color.a = opacity;
        spriteRenderer.color = color;
    }

}