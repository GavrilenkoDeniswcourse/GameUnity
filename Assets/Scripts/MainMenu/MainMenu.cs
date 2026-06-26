using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        DestroyOldManagers();

        // Удаляем сохранение
        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave();
        }
        SceneManager.LoadScene("Boot");
    }

    private void DestroyOldManagers()
    {
        // Удаляем игрока, если он есть
        Player oldPlayer = FindObjectOfType<Player>();
        if (oldPlayer != null)
            Destroy(oldPlayer.gameObject);

        // Удаляем SaveManager (он создастся заново)
        SaveManager oldSave = FindObjectOfType<SaveManager>();
        if (oldSave != null)
            Destroy(oldSave.gameObject);

        // Удаляем другие менеджеры, если нужно
        // CropManager, WaterManager и т.д.
    }

    public void Continue()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave())
        {
            SaveManager.Instance.LoadGame();
        }
        else
        {
            Debug.Log("Нет сохранения! Начните новую игру.");
        }
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}