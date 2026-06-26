using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    private string nextSpawnPoint = "DefaultSpawn";

    public void Awake()
    {
        Debug.Log("SceneTransitionManager Awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GoToScene(string sceneName, string spawnPointName)
    {
        nextSpawnPoint = spawnPointName;
        SceneManager.LoadScene(sceneName);
    }

    public string GetNextSpawnPoint()
    {
        return nextSpawnPoint;
    }

    public void ResetSpawnPoint()
    {
        nextSpawnPoint = "DefaultSpawn";
    }
}