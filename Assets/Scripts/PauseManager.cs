using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Or any pause key
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume time
        isPaused = false;
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause time
        isPaused = true;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        // Add your own quit logic here
        Debug.Log("Quit Game");
        Application.Quit();
    }
}


