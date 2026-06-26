using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuGame : MonoBehaviour
{
    public GameObject pausePanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
        }
    }

    public void CloseMenu()
    {
        pausePanel.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        CloseMenu(); //  закрыть меню паузы перед выходом
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        SaveManager.Instance.SaveGame();
    }

    public void Exit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}