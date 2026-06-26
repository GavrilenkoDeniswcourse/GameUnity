using UnityEngine;
using UnityEngine.UI;

public class CraftItemUI : MonoBehaviour
{
    public Image resultImage;
    public Text resultName;
    public Image ingredient1Image;
    public Image ingredient2Image;

    private SmeltingRecipeByID recipe;
    private CraftManager craftManager;

    public void Setup(SmeltingRecipeByID recipe, CraftManager manager)
    {
        this.recipe = recipe;
        this.craftManager = manager;

        IItem itemDB = FindObjectOfType<IItem>();

        // Результат
        Item result = itemDB.items[recipe.outputItemID];
        resultImage.sprite = result.image;
        resultName.text = result.name;

        // Первый ингредиент
        Item ing1 = itemDB.items[recipe.inputItem1ID];
        ingredient1Image.sprite = ing1.image;
        ingredient1Image.gameObject.SetActive(true);

        // Второй ингредиент (если есть)
        if (recipe.inputItem2ID != -1)
        {
            Item ing2 = itemDB.items[recipe.inputItem2ID];
            ingredient2Image.sprite = ing2.image;
            ingredient2Image.gameObject.SetActive(true);
        }
        else
        {
            ingredient2Image.gameObject.SetActive(false);
        }

        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnCraftClick);

        UpdateStatus();
    }

    private void UpdateStatus()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.interactable = craftManager.CanCraft(recipe);
    }

    private void OnCraftClick()
    {
        craftManager.Craft(recipe);
        UpdateStatus();
    }
}