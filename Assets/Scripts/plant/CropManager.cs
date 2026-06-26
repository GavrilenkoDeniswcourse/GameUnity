using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    [SerializeField] public CropData[] allCrops;
    [SerializeField] private Tilemap cropsTilemap;

    private Dictionary<Vector3Int, PlantedCrop> plantedCrops = new Dictionary<Vector3Int, PlantedCrop>();

    [System.Serializable]
    public class PlantedCrop
    {
        public CropData cropData;      // ссылка на данные культуры
        public int currentStage;       // текущая стадия (0, 1, 2...)
        public float plantTime;        // время посадки
        public Vector3Int position;     // позиция тайла
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "House")
        {
            FindHouseTilemaps();
        }
    }

    void Update()
    {
        if (cropsTilemap == null) return;

        foreach (var crop in new List<PlantedCrop>(plantedCrops.Values))
        {
            float timePassed = Time.time - crop.plantTime;
            int expectedStage = Mathf.FloorToInt(timePassed / crop.cropData.timePerStage);

            if (expectedStage > crop.currentStage && expectedStage < crop.cropData.growthStages.Length)
            {
                crop.currentStage = expectedStage;
                cropsTilemap.SetTile(crop.position, crop.cropData.growthStages[crop.currentStage]);
                
            }
        }
       
    }

    private void FindHouseTilemaps()
    {
        GameObject grid = GameObject.Find("Grid");

        if (grid != null)
        {
       
            cropsTilemap = GameObject.Find("Group")?.GetComponent<Tilemap>();
        }
    }
    public CropData GetCropBySeedID(int seedID)
    {
        foreach (CropData crop in allCrops)
        {
            if (crop.seedItemID == seedID)
            {
                return crop;
            }
        }
        return null;
    }

    public void PlantCrop(Vector3Int position, CropData cropData)
    {
        PlantedCrop newCrop = new PlantedCrop();
        newCrop.cropData = cropData;
        newCrop.currentStage = 0;
        newCrop.plantTime = Time.time;
        newCrop.position = position;
        plantedCrops[position] = newCrop;
    }

    public PlantedCrop GetCropAt(Vector3Int position)
    {
        plantedCrops.TryGetValue(position, out PlantedCrop crop);
        
        return crop;
        
    }

    public void RemoveCrop(Vector3Int position)
    {
        plantedCrops.Remove(position);
    }
}