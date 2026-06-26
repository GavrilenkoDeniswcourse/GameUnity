using UnityEngine;
using System.Collections.Generic;

public class Trader : MonoBehaviour
{
    public string Name;
    public List<ItemForSale> itemForSale = new List<ItemForSale>();
    public List<ItemSell> buyItemIds = new List<ItemSell>();
    public float PlayerRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    private TRadeUI tradeUI;
    private Transform player;
    private IItem itemDB;

    private void Start()
    {
        Debug.Log("Trader");
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        itemDB = FindFirstObjectByType<IItem>();

        GameObject panel = GameObject.Find("TradeCanvas");
        if (panel != null)
            tradeUI = FindObjectOfType<TRadeUI>(true);
        if (tradeUI == null)
        {
            Debug.LogError("TradeUI не найден на сцене!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (IsPlayerInRange())
                Interact();
        }
    }

    public void Interact()
    {
        NPCMovementWaypoints movement = GetComponent<NPCMovementWaypoints>();

        if (tradeUI == null) return;

        if (tradeUI.tradePanel.activeSelf)
        {
            tradeUI.CloseTrade();
            if (movement != null) movement.SetInteracting(false);
        }
        else
        {
            if (movement != null) movement.SetInteracting(true);
            tradeUI.OpenTrade(this);
        }
    }

    public bool IsPlayerInRange()
    {
        if (player == null)
        {
            Debug.LogError("Player = null!");
            return false;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log($"Расстояние до игрока: {distance}, радиус: {PlayerRange}");
        return distance <= PlayerRange;
    }

    // Методы для получения Item по ID
    public Item GetItemByID(int id)
    {
        if (itemDB != null && id >= 0 && id < itemDB.items.Count)
            return itemDB.items[id];
        return null;
    }
}

[System.Serializable]
public class ItemForSale
{
    public int itemID;
    public int priceBuy;
    public int quantity = -1;

    // Для удобства в инспекторе можно показать имя предмета
    public string itemName;
}

[System.Serializable]
public class ItemSell
{
    public int itemID;
    public int priceSell;

    public string itemName;
}