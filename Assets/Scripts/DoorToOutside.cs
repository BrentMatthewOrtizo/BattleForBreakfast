using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToOutside : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene(3);
    }

    public bool CanInteract()
    {
        return true;
    }
}