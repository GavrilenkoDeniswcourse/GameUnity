using UnityEngine;
using System.Collections.Generic;

public class CaveOreSpawner : MonoBehaviour
{
    [Header("Префабы руд")]
    public GameObject[] orePrefabs;  // сюда перетащи префабы руд

    [Header("Точки спавна")]
    public Transform[] spawnPoints;   // все возможные точки в пещере

    [Header("Настройки спавна")]
    public int minOres = 5;           // минимальное количество руд
    public int maxOres = 12;          // максимальное количество руд

    void Start()
    {
        SpawnOres();
    }

    void SpawnOres()
    {
        if (orePrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Нет префабов или точек спавна!");
            return;
        }

        // Случайное количество руд в этой пещере
        int oreCount = Random.Range(minOres, maxOres + 1);
        
        // Перемешиваем список точек, чтобы выбрать случайные
        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        
        for (int i = 0; i < oreCount && availablePoints.Count > 0; i++)
        {
            // Выбираем случайную точку из доступных
            int randomPointIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randomPointIndex];
            availablePoints.RemoveAt(randomPointIndex); // чтобы не спавнить дважды в одной точке
            
            // Выбираем случайную руду
            int randomOreIndex = Random.Range(0, orePrefabs.Length);
            GameObject oreToSpawn = orePrefabs[randomOreIndex];
            
            // Создаём руду в выбранной точке
            Instantiate(oreToSpawn, spawnPoint.position, Quaternion.identity);
        }
        
        Debug.Log($"Создано {oreCount} руд в пещере");
    }
}