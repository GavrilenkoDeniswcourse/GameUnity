using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    public List<SmeltingRecipeByID> craftRecipes = new List<SmeltingRecipeByID>();
    public Transform craftContainer;
    public GameObject craftItemPrefab;

    private InventoryDisplay playerInventory;
    private IItem itemDB;

    private void Start()
    {
        playerInventory = FindObjectOfType<InventoryDisplay>();
        itemDB = FindObjectOfType<IItem>();

        // Изначально панель скрыта
        gameObject.SetActive(false);
    }

    public void RefreshCraftPanel()
    {
        Debug.Log($"RefreshCraftPanel вызван. Рецептов: {craftRecipes.Count}");

        foreach (Transform child in craftContainer)
            Destroy(child.gameObject);

        foreach (var recipe in craftRecipes)
        {
            Debug.Log($"Создаю префаб для рецепта: ID={recipe.outputItemID}");
            GameObject newItem = Instantiate(craftItemPrefab, craftContainer);
            CraftItemUI craftUI = newItem.GetComponent<CraftItemUI>();
            if (craftUI != null)
                craftUI.Setup(recipe, this);
            else
                Debug.LogError("craftItemPrefab не имеет компонента CraftItemUI!");
        }
    }

    public bool CanCraft(SmeltingRecipeByID recipe)
    {
        if (playerInventory == null) return false;

        // Проверяем первый предмет
        int count1 = playerInventory.GetItemCount(recipe.inputItem1ID);
        if (count1 < 1) return false;

        // Проверяем второй предмет, если есть
        if (recipe.inputItem2ID != -1)
        {
            int count2 = playerInventory.GetItemCount(recipe.inputItem2ID);
            if (count2 < 1) return false;
        }

        return true;
    }

    public void Craft(SmeltingRecipeByID recipe)
    {
        if (!CanCraft(recipe)) return;

        // Удаляем ингредиенты
        playerInventory.RemoveItem(recipe.inputItem1ID, 1);

        if (recipe.inputItem2ID != -1)
            playerInventory.RemoveItem(recipe.inputItem2ID, 1);

        // Добавляем результат
        Item result = itemDB.items[recipe.outputItemID];
        playerInventory.SearchForSameItem(result, 1);

        // Обновляем UI
        RefreshCraftPanel();
        playerInventory.UpdateInventory();
    }
}