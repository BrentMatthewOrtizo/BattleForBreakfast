using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToStore : MonoBehaviour, IInteractable
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
