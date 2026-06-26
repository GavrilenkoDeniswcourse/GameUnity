using UnityEngine;
using UnityEngine.UI;

public class TradeItemSellUI : MonoBehaviour
{
    public Image itemImage;
    public Text itemName;
    public Text itemPrice;
    public Text amount;
    public Button plus;
    public Button minus;

    private ItemSell itemForSale;
    private Trader currentTrader;
    private TRadeUI ui;
    private InventoryItem inventoryItem;
    private Item cachedItem;

    private int currentCount;

    public void SetupSell(ItemSell newItem, Trader trader, TRadeUI ui, InventoryItem invItem)
    {
        itemForSale = newItem;
        currentTrader = trader;
        this.ui = ui;
        inventoryItem = invItem;
        currentCount = 0;

        // Получаем Item по ID из базы данных торговца
        cachedItem = currentTrader.GetItemByID(newItem.itemID);

        if (itemImage != null && cachedItem != null && cachedItem.image != null)
            itemImage.sprite = cachedItem.image;

        if (itemName != null && cachedItem != null)
            itemName.text = cachedItem.name;

        if (itemPrice != null)
            itemPrice.text = itemForSale.priceSell.ToString();

        if (amount != null)
            amount.text = "0";

        if (plus != null)
            plus.onClick.AddListener(PlusCount);

        if (minus != null)
            minus.onClick.AddListener(MinusCount);

        if (GetComponent<Button>() != null)
            GetComponent<Button>().onClick.AddListener(OnSellButtonClick);
    }

    private void PlusCount()
    {
        if (currentCount < inventoryItem.count)
        {
            currentCount++;
            amount.text = currentCount.ToString();
        }
    }

    private void MinusCount()
    {
        if (currentCount > 0)
        {
            currentCount--;
            amount.text = currentCount.ToString();
        }
    }

    private void OnSellButtonClick()
    {
        if (currentCount <= 0) return;
        if (currentCount > inventoryItem.count) return;

        Wallet wallet = FindFirstObjectByType<Wallet>();
        InventoryDisplay inventory = FindFirstObjectByType<InventoryDisplay>();

        if (wallet == null || inventory == null) return;

        wallet.AddCoins(itemForSale.priceSell * currentCount);
        inventory.RemoveItem(inventoryItem.id, currentCount);

        ui.RefreshSellTab(); // обновляем вкладку продажи, а не покупки
    }
}