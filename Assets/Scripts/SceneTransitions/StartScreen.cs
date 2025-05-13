using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(1);
    }
    
    public void OnGameButtonClicked()
    {
        SceneManager.LoadScene(4);
    }
    
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
    
}
