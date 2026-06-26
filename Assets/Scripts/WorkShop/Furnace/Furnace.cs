using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    public List<SmeltingRecipeByID> recipes = new List<SmeltingRecipeByID>();
    private bool isSmelting;
    private float currentSmeltTime;
    private InventoryDisplay playerInventory;
    private IItem itemDB;
    private Item pendingOutput;
    private Item cachedOutputItem;
    public Item CompletedOutput => cachedOutputItem;
    public System.Action OnOutputReady;
    public System.Action OnOutputTaken;

    private void Start()
    {
        playerInventory = FindObjectOfType<InventoryDisplay>();
        itemDB = FindObjectOfType<IItem>();
    }

    void Update()
    {
        if (isSmelting)
        {
            currentSmeltTime -= Time.deltaTime;
            if (currentSmeltTime <= 0)
            {
                isSmelting = false;
                cachedOutputItem = pendingOutput;
                pendingOutput = null;
                OnOutputReady?.Invoke();
            }
        }
    }

    public bool IsSmelting()
    {
        return isSmelting;
    }

    public Item GetItemByID(int id)
    {
        if (itemDB != null && id >= 0 && id < itemDB.items.Count)
            return itemDB.items[id];
        return null;
    }

    public bool CanSmelt(SmeltingRecipeByID recipe)
    {
        if (isSmelting) return false;
        if (playerInventory == null) return false;

        Item inputItem1 = GetItemByID(recipe.inputItem1ID);
        if (inputItem1 == null)
        {
            Debug.LogError($"Предмет с ID {recipe.inputItem1ID} не найден в IItem!");
            return false;
        }

        int count1 = playerInventory.GetItemCount(inputItem1.id);
        if (count1 < 1) return false;

        if (recipe.inputItem2ID != -1)
        {
            Item inputItem2 = GetItemByID(recipe.inputItem2ID);
            if (inputItem2 == null)
            {
                Debug.LogError($"Предмет с ID {recipe.inputItem2ID} не найден в IItem!");
                return false;
            }

            int count2 = playerInventory.GetItemCount(inputItem2.id);
            if (count2 < 1) return false;
        }

        return true;
    }

    public void Smelt(SmeltingRecipeByID recipe)
    {
        if (isSmelting) return;
        if (!CanSmelt(recipe)) return;

        Item inputItem1 = GetItemByID(recipe.inputItem1ID);
        playerInventory.RemoveItem(inputItem1.id, 1);

        if (recipe.inputItem2ID != -1)
        {
            Item inputItem2 = GetItemByID(recipe.inputItem2ID);
            playerInventory.RemoveItem(inputItem2.id, 1);
        }

        pendingOutput = GetItemByID(recipe.outputItemID);
        currentSmeltTime = recipe.smeltTime;
        isSmelting = true;
    }

    public void TakeResult()
    {
        if (cachedOutputItem == null) return;

        playerInventory.SearchForSameItem(cachedOutputItem, 1);
        cachedOutputItem = null;
        OnOutputTaken?.Invoke();
    }
}