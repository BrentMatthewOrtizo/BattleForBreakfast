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
        SceneManager.LoadScene(2);
    }
    
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
    
}
