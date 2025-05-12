using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LadderToDS : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene(2);
    }

    public bool CanInteract()
    {
        return true;
    }
}
