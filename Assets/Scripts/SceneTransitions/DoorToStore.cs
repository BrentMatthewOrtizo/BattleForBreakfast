using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToStore : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene(4);
    }

    public bool CanInteract()
    {
        return true;
    }
}
