using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    public Image itemImage1;
    public Image itemImage2;
    public Image resultImage;
    public Text itemNameText;
    private Button button;
    private SmeltingRecipeByID recipe;
    private Furnace furnace;
    private Item cachedInputItem1;
    private Item cachedInputItem2;
    private Item cachedOutputItem;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
            Debug.LogError($"RecipeUI: Button не найден на {gameObject.name}!");
    }

    public void Setup(SmeltingRecipeByID recipe, Furnace furnace)
    {
        if (recipe == null)
        {
            Debug.LogError("RecipeUI.Setup: recipe = null!");
            return;
        }

        if (furnace == null)
        {
            Debug.LogError("RecipeUI.Setup: furnace = null!");
            return;
        }

        if (button == null)
        {
            Debug.LogError("RecipeUI.Setup: button = null!");
            return;
        }

        this.recipe = recipe;
        this.furnace = furnace;

        // Получаем предметы по ID
        cachedInputItem1 = furnace.GetItemByID(recipe.inputItem1ID);
        cachedOutputItem = furnace.GetItemByID(recipe.outputItemID);

        if (recipe.inputItem2ID != -1)
            cachedInputItem2 = furnace.GetItemByID(recipe.inputItem2ID);

        // Первый предмет
        if (itemImage1 != null && cachedInputItem1 != null)
            itemImage1.sprite = cachedInputItem1.image;

        // Результат
        if (resultImage != null && cachedOutputItem != null)
            resultImage.sprite = cachedOutputItem.image;

        // Название предмета
        if (itemNameText != null && cachedOutputItem != null)
            itemNameText.text = cachedOutputItem.name;

        // Второй предмет — скрываем, если нет
        if (recipe.inputItem2ID != -1 && cachedInputItem2 != null)
        {
            itemImage2.gameObject.SetActive(true);
            itemImage2.sprite = cachedInputItem2.image;
        }
        else
        {
            itemImage2.sprite = null;
            itemImage2.gameObject.SetActive(false);
        }

        UpdateStatus();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void UpdateStatus()
    {
        if (button == null || furnace == null || recipe == null) return;
        button.interactable = furnace.CanSmelt(recipe) && !furnace.IsSmelting();
    }

    private void OnClick()
    {
        if (furnace == null || recipe == null) return;

        Item outputItem = furnace.GetItemByID(recipe.outputItemID);
        Debug.Log($"Клик по рецепту: {(outputItem != null ? outputItem.name : "Unknown")}");
        furnace.Smelt(recipe);
        UpdateStatus();
    }
}