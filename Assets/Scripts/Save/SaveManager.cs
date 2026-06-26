using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/save.json";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            data.playerHealth = player.CurrentHealth;
        }

        Wallet wallet = FindObjectOfType<Wallet>();
        if (wallet != null)
        {
            data.coins = wallet.GetCoins();
        }

        InventoryDisplay inventory = FindObjectOfType<InventoryDisplay>();
        if (inventory != null)
        {
            foreach (var item in inventory.items)
            {
                if (item.id != 0)
                {
                    InventorySlotData slotData = new InventorySlotData();
                    slotData.itemId = item.id;
                    slotData.count = item.count;
                    data.inventoryItems.Add(slotData);
                }
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Игра сохранена в: " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.ResetSpawnPoint();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Boot");
            StartCoroutine(LoadAfterSceneLoad(data));
        }
        else
        {
            Debug.Log("Сохранение не найдено!");
        }
    }

    private System.Collections.IEnumerator LoadAfterSceneLoad(SaveData data)
    {
        yield return null;

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetHealth(data.playerHealth);
        }

        Wallet wallet = FindObjectOfType<Wallet>();
        if (wallet != null)
        {
            wallet.SetCoins(data.coins);
        }

        InventoryDisplay inventory = FindObjectOfType<InventoryDisplay>();
        if (inventory != null)
        {
            // Очищаем инвентарь
            for (int i = 0; i < inventory._maxCount; i++)
            {
                inventory.AddItem(i, inventory._iitem.items[0], 0);
            }

            // Загружаем сохранённые предметы
            foreach (var slotData in data.inventoryItems)
            {
                Item item = inventory._iitem.items[slotData.itemId];
                inventory.SearchForSameItem(item, slotData.count);
            }
            inventory.UpdateInventory();
        }

        Debug.Log("Игра загружена!");
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Сохранение удалено!");
        }
    }
}