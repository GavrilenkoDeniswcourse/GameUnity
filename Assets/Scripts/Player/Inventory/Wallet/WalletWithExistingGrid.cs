using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletWithExistingGrid : MonoBehaviour
{
    public Image walletImag;
    public GameObject digitPrefab;
    public RectTransform cellsContainer;
    public Wallet walletScript;

    [SerializeField] private float cellWidth = 40f;
    [SerializeField] private float cellHeight = 50f;
    [SerializeField] private Vector2 firstCellPosition = new Vector2(-80f, -12f);

    private Text[] digitText;


    void Update()
    {
        
    }

    public void SetCoins(int amount)
    {
        // Удаляем старые цифры
        foreach (Transform child in cellsContainer)
        {
            Destroy(child.gameObject);
        }

        string coinString = amount.ToString();
        float digitWidth = 40f; // Ширина одной цифры (как у DigitTemplate)
        float totalWidth = coinString.Length * digitWidth;

        // Стартовая позиция, чтобы весь блок был по центру
        float startX = -totalWidth / 2f;

        for (int i = 0; i < coinString.Length; i++)
        {
            char digitChar = coinString[i];
            Transform digit = Instantiate(digitPrefab, cellsContainer).transform;

            RectTransform rect = digit.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(firstCellPosition.x + i * cellWidth, firstCellPosition.y, 0);

            // Устанавливаем текст цифры
            TextMeshProUGUI textComponent = digit.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = digitChar.ToString();
            }
            else
            {
                Text oldText = digit.GetComponent<Text>();
                if (oldText != null) oldText.text = digitChar.ToString();
            }
        }
    }
}
