using UnityEngine;
using UnityEngine.UI;

public class TRadeUI : MonoBehaviour
{
    public GameObject tradePanel;
    public Transform itemsContainer;
    public Transform itemContainerSell;
    public GameObject itemPrefabBuy;
    public GameObject itemPrefabSell;
    private Trader currentTrader;
    public Button closeButton;
    public GameObject buyContainer;
    public GameObject sellContainer;

    private void Start()
    {
        Debug.Log("TRadeUI");
        tradePanel.SetActive(false);
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTrade);
        }
    }

    public void OpenTrade(Trader trader)
    {
        Debug.Log("OpenTrade вызван");
        currentTrader = trader;
        gameObject.SetActive(true);
        RefreshSellTab();
        RefreshItems();
    }

    public void ShowBuyTab()
    {
        RefreshItems();
        buyContainer.SetActive(true);
        sellContainer.SetActive(false);
    }

    public void ShowSellTab()
    {
        RefreshSellTab();
        sellContainer.SetActive(true);
        buyContainer.SetActive(false);
    }

    public void CloseTrade()
    {
        gameObject.SetActive(false);
        currentTrader = null;
    }

    public void RefreshItems()
    {
        ClearItems();
        if (currentTrader == null) return;
        foreach (var itemsForSale in currentTrader.itemForSale)
        {
            AddItem(itemsForSale);
        }
    }

    public void ClearItems()
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearSellContainer()
    {
        foreach (Transform child in itemContainerSell)
        {
            Destroy(child.gameObject);
        }
    }

    public void RefreshSellTab()
    {
        ClearSellContainer();
        if (currentTrader == null) return;

        InventoryDisplay inventory = FindFirstObjectByType<InventoryDisplay>();
        if (inventory == null) return;

        foreach (var invItem in inventory.items)
        {
            if (invItem.id == 0) continue;

            foreach (var itemSell in currentTrader.buyItemIds)
            {
                if (itemSell.itemID == invItem.id)
                {
                    AddSellItem(itemSell, invItem);
                    break;
                }
            }
        }
    }

    private void AddItem(ItemForSale itemsForSale)
    {
        GameObject newItem = Instantiate(itemPrefabBuy, itemsContainer);
        TradeItemUI itemUI = newItem.GetComponent<TradeItemUI>();
        if (itemUI != null)
        {
            itemUI.Setup(itemsForSale, currentTrader, this);
        }
    }

    private void AddSellItem(ItemSell itemSell, InventoryItem invItem)
    {
        GameObject newItem = Instantiate(itemPrefabSell, itemContainerSell);
        TradeItemSellUI itemSellUI = newItem.GetComponent<TradeItemSellUI>();
        if (itemSellUI != null)
        {
            itemSellUI.SetupSell(itemSell, currentTrader, this, invItem);
        }
    }
}