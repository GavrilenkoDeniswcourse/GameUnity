using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{
    [Header("UI")]
    public Transform recipeContainer;
    public GameObject recipePrefab;
    public Image resultImage;
    public Button takeButton;
    public Button closeButton;

    private Furnace furnace;

    private void Start()
    {
        gameObject.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (takeButton != null)
            takeButton.onClick.AddListener(TakeResult);
    }

    public void Init(Furnace furnace)
    {
        this.furnace = furnace;

        if (furnace != null)
        {
            furnace.OnOutputReady += RefreshResult;
            furnace.OnOutputTaken += RefreshResult;
        }

        RefreshRecipes();
        RefreshResult();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        RefreshRecipes();
        RefreshResult();
    }

    public void Close()
    {
        gameObject.SetActive(false);

        if (furnace != null)
        {
            furnace.OnOutputReady -= RefreshResult;
            furnace.OnOutputTaken -= RefreshResult;
        }
    }

    private void RefreshRecipes()
    {
        if (recipeContainer == null || furnace == null) return;

        foreach (Transform child in recipeContainer)
            Destroy(child.gameObject);

        foreach (var recipe in furnace.recipes)
        {
            if (recipe == null) continue;

            GameObject obj = Instantiate(recipePrefab, recipeContainer);
            RecipeUI recipeUI = obj.GetComponent<RecipeUI>();
            if (recipeUI != null)
                recipeUI.Setup(recipe, furnace);
        }
    }

    private void RefreshResult()
    {
        if (takeButton == null || resultImage == null) return;

        if (furnace != null && furnace.CompletedOutput != null)
        {
            takeButton.gameObject.SetActive(true);
            resultImage.sprite = furnace.CompletedOutput.image;
            takeButton.interactable = true;
        }
        else
        {
            takeButton.gameObject.SetActive(false);
            resultImage.sprite = null;
            takeButton.interactable = false;
        }
    }

    private void TakeResult()
    {
        if (furnace != null)
            furnace.TakeResult();
    }
}