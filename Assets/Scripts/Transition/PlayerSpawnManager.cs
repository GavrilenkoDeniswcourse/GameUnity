using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== PlayerSpawnManager Start ===");

        string spawnName = SceneTransitionManager.Instance.GetNextSpawnPoint();
        Debug.Log("Ищу точку с именем: '" + spawnName + "'");

        GameObject spawnPoint = GameObject.Find(spawnName);
        if (spawnPoint != null)
        {
            Debug.Log("Точка найдена! Перемещаю игрока");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = spawnPoint.transform.position;
        }
        else
        {
            Debug.LogWarning("Точка '" + spawnName + "' не найдена!");
        }
    }
}