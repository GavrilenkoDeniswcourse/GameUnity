using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WaterManager : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private TileBase tileGrassTile;
    [SerializeField] private TileBase tileSoileTile;
    [SerializeField] private TileBase tileWaterTile;
    [SerializeField] private float dryTile = 5f;
    [SerializeField] private float ñompactedTile = 10f;
    [SerializeField] private CropManager cropsManager;

    private float _checkPosition;

    private Dictionary<Vector3Int, float> waterKey = new Dictionary<Vector3Int, float>();
    private Dictionary<Vector3Int, float> landKey = new Dictionary<Vector3Int, float>();

    List<Vector3Int> waterList = new List<Vector3Int>();
    List<Vector3Int> landList = new List<Vector3Int>();


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
        if(tileMap == null) return;
        CheckWaterList();
        CheckLandList();

        waterList.Clear();
        landList.Clear();
    }

    private void FindHouseTilemaps()
    {
        GameObject grid = GameObject.Find("Grid");

        if (grid != null)
        {

            tileMap = GameObject.Find("Grass")?.GetComponent<Tilemap>();
        }
    }
    public void WaterTile(Vector3Int position)
    {
        TileBase currentTile = tileMap.GetTile(position);
        

        if (currentTile == tileSoileTile)
        {
            if (landKey.ContainsKey(position)) 
             landKey.Remove(position);

            tileMap.SetTile(position, tileWaterTile);
            waterKey[position] = Time.time;
            
        }
    }

    public void LandTile(Vector3Int position) 
    {
        TileBase currentTile = tileMap.GetTile(position);
        if (currentTile == tileGrassTile)
        {
            tileMap.SetTile(position, tileSoileTile);
            
            landKey[position] = Time.time;
        }

    }

    private void CheckLandList()
    {
        foreach (var landPair in landKey)
        {
            float timePollLand = Time.time - landPair.Value;
            if (ñompactedTile <= timePollLand)
            {
                if (cropsManager.GetCropAt(landPair.Key) == null)
                {
                    tileMap.SetTile(landPair.Key, tileGrassTile);
                    landList.Add(landPair.Key);
                }
            }

        }
        foreach (Vector3Int landenKey in landList)
        {
            landKey.Remove(landenKey);
        }
    }

    private void CheckWaterList()
    {
        foreach (var waterPair in waterKey)
        {
            float timePoll = Time.time - waterPair.Value;

            if (dryTile <= timePoll)
            {
                tileMap.SetTile(waterPair.Key, tileSoileTile);
                landKey[waterPair.Key] = Time.time;
                waterList.Add(waterPair.Key);
               
            }

        }
        foreach (Vector3Int key in waterList)
        {
            waterKey.Remove(key);
        }
    }
}
