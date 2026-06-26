using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int playerHealth;
    public int coins;

    public List<InventorySlotData> inventoryItems = new List<InventorySlotData>();
}

[System.Serializable]
public class InventorySlotData
{
    public int itemId;
    public int count;
}