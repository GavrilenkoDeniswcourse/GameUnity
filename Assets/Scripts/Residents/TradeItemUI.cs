using UnityEngine;
using UnityEngine.UI;

public class TradeItemUI : MonoBehaviour
{
    public Image itemImage;
    public Text itemName;
    public Text itemPrice;

    private ItemForSale itemForSale;
    private Trader currentTrader;
    private TRadeUI traderUI;
    private Item cachedItem;

    public void Setup(ItemForSale newItem, Trader trader, TRadeUI ui)
    {
        itemForSale = newItem;
        currentTrader = trader;
        traderUI = ui;

        // Получаем Item по ID из базы данных торговца
        cachedItem = currentTrader.GetItemByID(newItem.itemID);

        // Заполняем UI
        if (itemImage != null && cachedItem != null && cachedItem.image != null)
            itemImage.sprite = cachedItem.image;

        if (itemName != null && cachedItem != null)
            itemName.text = cachedItem.name;

        if (itemPrice != null)
            itemPrice.text = itemForSale.priceBuy.ToString();

        if (GetComponent<Button>() != null)
            GetComponent<Button>().onClick.AddListener(OnBuyButtonClick);
    }

    private void OnBuyButtonClick()
    {
        if (cachedItem == null)
        {
            Debug.LogError("Item не найден по ID!");
            return;
        }

        Wallet wallet = FindFirstObjectByType<Wallet>();
        if (wallet == null)
        {
            Debug.LogError("Wallet ne nayden");
            return;
        }

        if (wallet.GetCoins() >= itemForSale.priceBuy)
        {
            InventoryDisplay inventory = FindFirstObjectByType<InventoryDisplay>();
            if (inventory != null)
            {
                inventory.SearchForSameItem(cachedItem, 1);
                wallet.SpendCoins(itemForSale.priceBuy);
                Debug.Log($"Куплено: {cachedItem.name} за {itemForSale.priceBuy} монет");
            }
        }
        else
        {
            Debug.Log("Не хватает монет!");
        }
    }
}