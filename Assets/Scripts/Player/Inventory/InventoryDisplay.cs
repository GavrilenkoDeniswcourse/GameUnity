using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public GameObject gameObjShow;
    public GameObject InventoryMainObject;

    public int _maxCount;

    public IItem _iitem;

    public Camera _camera;
    public EventSystem _es;

    public int currentId = -1;
    public InventoryItem currentItem;

    public RectTransform _movingObject;
    public Vector3 offset;

    public GameObject backGround;

    public GameObject craftPanel;

    public CraftManager _craftManager;

    private void Start()
    {
        AddGraphics(); // всегда создаём слоты

        backGround.SetActive(false);

        AddItem(1, _iitem.items[1], 1);
        AddItem(2, _iitem.items[2], 1);
        AddItem(3, _iitem.items[3], 1);
        AddItem(4, _iitem.items[4], 1);
        AddItem(5, _iitem.items[6], 1);
        AddItem(6, _iitem.items[7], 5);
        AddItem(7, _iitem.items[11], 1);
        AddItem(8, _iitem.items[14], 1);
        AddItem(9, _iitem.items[15], 4);

        UpdateInventory();
    }

    private void Update()
    {
        if (currentId != -1)
            MoveObject();

        if (Input.GetKeyDown(KeyCode.I))
        {
            backGround.SetActive(!backGround.activeSelf);
            craftPanel.SetActive(backGround.activeSelf);
            if (backGround.activeSelf)
            { 
                UpdateInventory();
                if (_craftManager != null)
                {
                    Debug.Log("Вызываю RefreshCraftPanel()");
                    _craftManager.RefreshCraftPanel();
                }
                else
                {
                    Debug.LogError("craftManager = null!");
                }
            }
        }
    }

    // ========================
    // ДОБАВЛЕНИЕ ПРЕДМЕТА
    // ========================
    public void AddItem(int index, Item item, int count)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogError($"AddItem: индекс {index} вне диапазона!");
            return;
        }

        items[index].id = item.id;
        items[index].count = count;

        Transform itemImageTransform = items[index].itemGameObj.transform.Find("ImageItem");
        if (itemImageTransform != null)
        {
            Image itemImage = itemImageTransform.GetComponent<Image>();
            if (itemImage != null)
            {
                if (item.id != 0)
                {
                    itemImage.sprite = _iitem.items[items[index].id].image;
                    itemImage.gameObject.SetActive(true);
                }
                else 
                {
                    itemImage.sprite = null;
                    itemImage.gameObject.SetActive(false);
                }
                
            }
        }
        var txt = items[index].itemGameObj.GetComponentInChildren<Text>();

       

        if (count > 1 && item.id != 0)
            txt.text = count.ToString();
        else
            txt.text = "";
    }

    // ========================
    // ДОБАВЛЕНИЕ В СТАК
    // ========================
    public void SearchForSameItem(Item item, int count)
    {
        for (int i = 0; i < _maxCount; i++)
        {
            if (items[i].id == item.id)
            {
                int space = 50 - items[i].count;

                if (space > 0)
                {
                    int add = Mathf.Min(space, count);
                    items[i].count += add;
                    count -= add;

                    if (count <= 0)
                        break;
                }
            }
        }

        // если осталось — кладём в пустой слот
        if (count > 0)
        {
            for (int i = 0; i < _maxCount; i++)
            {
                if (items[i].id == 0)
                {
                    AddItem(i, item, count);
                    break;
                }
            }
        }

        UpdateInventory();
    }

    // ========================
    // УДАЛЕНИЕ ПРЕДМЕТА (FIXED)
    // ========================
    public void RemoveItem(int id, int count)
    {
        int remaining = count;

        for (int i = 0; i < _maxCount && remaining > 0; i++)
        {
            if (items[i].id == id)
            {
                if (items[i].count > remaining)
                {
                    items[i].count -= remaining;
                    remaining = 0;
                }
                else
                {
                    remaining -= items[i].count;

                    // очищаем слот
                    AddItem(i, _iitem.items[0], 0);
                }
            }
        }

        if (remaining > 0)
        {
            Debug.LogWarning($"RemoveItem: не хватило предмета id {id}, не удалено {remaining}");
        }

        UpdateInventory();
    }

    // ========================
    // ПОЛУЧЕНИЕ КОЛ-ВА
    // ========================
    public int GetItemCount(int itemId)
    {
        int total = 0;

        for (int i = 0; i < _maxCount; i++)
        {
            if (items[i].id == itemId)
                total += items[i].count;
        }

        return total;
    }

    // ========================
    // СОЗДАНИЕ СЛОТОВ
    // ========================
    public void AddGraphics()
    {
        items.Clear();

        for (int i = 0; i < _maxCount; i++)
        {
            GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform);
            newItem.name = i.ToString();

            InventoryItem ii = new InventoryItem();
            ii.itemGameObj = newItem;
            ii.id = 0;
            ii.count = 0;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            Button btn = newItem.GetComponent<Button>();
            btn.onClick.AddListener(delegate { SelectObject(); });

            items.Add(ii);
        }
    }

    // ========================
    // ОБНОВЛЕНИЕ UI
    // ========================
    public void UpdateInventory()
    {
        for (int i = 0; i < _maxCount; i++)
        {
            var item = items[i];

            // Пропускаем, если объект слота не существует
            if (item.itemGameObj == null) continue;

            // Ищем дочерний объект ImageItem
            Transform itemImageTransform = item.itemGameObj.transform.Find("ImageItem");

            if (itemImageTransform != null)
            {
                Image itemImage = itemImageTransform.GetComponent<Image>();

                // ВАЖНО: проверяем, что itemImage не null
                if (itemImage != null)
                {
                    if (item.id != 0)
                    {
                        itemImage.sprite = _iitem.items[item.id].image;
                        itemImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        itemImage.sprite = null;
                        itemImage.gameObject.SetActive(false);
                    }
                }
            }

            // Текст количества
            var txt = item.itemGameObj.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = (item.count > 1 && item.id != 0) ? item.count.ToString() : "";
            }
        }
    }

    // ========================
    // DRAG & DROP
    // ========================
    public void SelectObject()
    {
        int selectedIndex = int.Parse(_es.currentSelectedGameObject.name);

        if (currentId == -1)
        {
            currentId = selectedIndex;
            currentItem = CopyInventoryItem(items[currentId]);

            _movingObject.gameObject.SetActive(true);
            _movingObject.GetComponent<Image>().sprite = _iitem.items[currentItem.id].image;

            AddItem(currentId, _iitem.items[0], 0);
        }
        else
        {
            InventoryItem target = items[selectedIndex];

            if (currentItem.id != target.id)
            {
                AddInventoryItem(currentId, target);
                AddInventoryItem(selectedIndex, currentItem);
            }
            else
            {
                int total = target.count + currentItem.count;

                if (total <= 50)
                {
                    target.count = total;
                    AddItem(currentId, _iitem.items[0], 0);
                }
                else
                {
                    target.count = 50;
                    AddItem(currentId, _iitem.items[target.id], total - 50);
                }
            }

            currentId = -1;
            _movingObject.gameObject.SetActive(false);
        }

        UpdateInventory();
    }

    public InventoryItem CopyInventoryItem(InventoryItem old)
    {
        return new InventoryItem
        {
            id = old.id,
            count = old.count,
            itemGameObj = old.itemGameObj
        };
    }

    public void AddInventoryItem(int index, InventoryItem invItem)
    {
        if (index < 0 || index >= items.Count) return;

        items[index].id = invItem.id;
        items[index].count = invItem.count;

        Transform itemImageTransform = items[index].itemGameObj.transform.Find("ImageItem");
        if (itemImageTransform != null)
        {
            Image itemImage = itemImageTransform.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = (items[index].id != 0) ? _iitem.items[items[index].id].image : null;
            }
        }
        var txt = items[index].itemGameObj.GetComponentInChildren<Text>();

        if (invItem.count > 1 && invItem.id != 0)
            txt.text = invItem.count.ToString();
        else
            txt.text = "";
    }

    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = 0;
        _movingObject.position = pos;
    }

    public void RemoveRandomItems(int count)
    {
        // Собираем список слотов, где есть предметы
        List<int> nonEmptySlots = new List<int>();
        for (int i = 0; i < items.Count; i++) 
        {
            if (items[i].id != 0)
            {
                nonEmptySlots.Add(i);
            }
        
        }

        // Удаляем случайные слоты
        for (int i = 0; i < count && nonEmptySlots.Count > 0; i++) 
        {
            int randomIndex = UnityEngine.Random.Range(0, nonEmptySlots.Count);
            int slotIndex  = nonEmptySlots[randomIndex];

            // Очищаем слот
            AddItem(slotIndex, _iitem.items[0], 0);

            // Удаляем из списка, чтобы не удалить один и тот же слот дважды
            nonEmptySlots.RemoveAt(randomIndex);
        }

        UpdateInventory();
    }

    // Добавь этот метод в InventoryDisplay
    public void TrashCurrentItem()
    {
        if (currentId == -1)
        {
            Debug.Log("Сначала выбери предмет");
            return;
        }

        if (currentId >= 0 && currentId < items.Count && items[currentId].id != 0)
        {
            items[currentId].id = 0;
            items[currentId].count = 0;
            AddItem(currentId, _iitem.items[0], 0);
            currentId = -1;
            UpdateInventory();
            Debug.Log("Предмет удалён");
        }
    }
}