using UnityEngine;
using UnityEngine.UI;

public class Wallet : MonoBehaviour
{
    public InventoryDisplay inventoryDisplay;
    [SerializeField] private int coins;
   
    public Text coinsText;
    public WalletWithExistingGrid walletDisplay;

    private void Start()
    {
        UpdateUI();
        walletDisplay.SetCoins(coins);
    }

    private void Update()
    {
        walletDisplay.SetCoins(coins);
    }

    public int GetCoins()
    {
        return coins;
    }

    public void AddCoins(int amount)
    {
        
        coins += amount;
        UpdateUI();
        
    }

    public bool SpendCoins(int amount)
    {
        

        if (amount <= coins) 
        { 
         coins -= amount;
            UpdateUI();
            return true;
        }
        else return false;

        
    }

    public void SetCoins(int amount)
    {
        coins = amount;
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = coins.ToString();

        }
    }

    

}
