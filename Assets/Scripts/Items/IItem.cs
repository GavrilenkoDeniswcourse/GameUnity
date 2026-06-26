using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class IItem : MonoBehaviour 
{
    public List<Item> items = new List<Item>();

}


[System.Serializable]
public class Item
{

    public int id;
    public string name;
    public Sprite image;
   
    public int damage;

    public ToolType toolType;
    
    public enum ToolType
    {
        None,
        Axe,
        Pickaxe,
        WateringCan,
        Hoe,
        Weapon,
        Resource,
        Seeds,
        Serp 
    }

    
}


[System.Serializable]
public class CropData
{
    public string cropName;           // "Carrot", "Potato"
    public int seedItemID;            // ID семян в IItem
    public TileBase[] growthStages;   // массив тайлов (стадия1, стадия2, стадия3, готовый урожай)
    public float timePerStage;        // время на одну стадию (в секундах)
    public int harvestItemID;         // что даёт при сборе
    public int harvestCount;          // сколько даёт
}
