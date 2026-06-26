using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static CropManager;
using static UnityEngine.UI.Image;

public class InventoriQuickAccess : MonoBehaviour
{
    public InventoryDisplay _mainInventory;
    public List<GameObject> slotObject = new List<GameObject>();
    public int quickAccessCount = 10;
    public GameObject slotPrefab;
    public IItem _iitem;
    public GameObject InventoryMainObject;
    private Player _player;
    private int _slotIndex;
    private Item.ToolType _tool;
    private int _currentCount;

    public Animator animator;

    private const string TOOLType = "toolType";
    private const string UseTool = "UseTool";

    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private LayerMask hitLayerEnemy;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject _effectPrefab;

    [SerializeField] private Tilemap worldTilemap;
    [SerializeField] private Tilemap cropsTilemap;
    [SerializeField] private TileBase grass;
    [SerializeField] private TileBase pathTile; 
    [SerializeField] private TileBase tilledSoilTile;
    [SerializeField] private TileBase tilledSoilTileNot;
    [SerializeField] private TileBase wateredTile;
    [SerializeField] private TileBase seedStage1Tile;
    [SerializeField] private CropManager cropManager;
    [SerializeField] private Transform player;
    [SerializeField] private WaterManager waterManager;

    

    private Vector2 debugAttackCenter;
    private Vector2 debugAttackSize;

    private void Start()
    {
        _slotIndex = 0;
        CreateQuickAccessSlot();
        //ColorSlot();
        UpdateQuickAccess();

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "House")
        {
            FindHouseTilemaps();
        }
    }

    private void Update()
    {
        UpdateQuickAccess();
        SwitchQuickAccess();
        ClickControl();
       
    }

    private void FindHouseTilemaps()
    {
        GameObject grid = GameObject.Find("Grid");

        if(grid != null)
        {
            worldTilemap = GameObject.Find("Grass")?.GetComponent<Tilemap>();
            cropsTilemap = GameObject.Find("Group")?.GetComponent<Tilemap>();
        }

        if (worldTilemap == null)
            Debug.LogWarning("WorldTilemap не найден в сцене House");

        if (cropsTilemap == null)
            Debug.LogWarning("CropsTilemap не найден в сцене House");
    }

private void CreateQuickAccessSlot()
    {
        for (int i = 0; i < quickAccessCount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, InventoryMainObject.transform);
            newSlot.name = i.ToString();
            slotObject.Add(newSlot);
        }
    }

    public void UpdateQuickAccess()
    {
        for (int i = 0; i < quickAccessCount; i++)
        {
            // Ищем дочерний объект "ImageQuick" для иконки предмета
            Transform imageQuickTransform = slotObject[i].transform.Find("ImageQuick");
            if (imageQuickTransform == null) continue;

            UnityEngine.UI.Image itemIcon = imageQuickTransform.GetComponent<UnityEngine.UI.Image>();
            if (itemIcon == null) continue;

            // Фон слота (сам слот)
            UnityEngine.UI.Image slotBackground = slotObject[i].GetComponent<UnityEngine.UI.Image>();

            // Обновляем иконку предмета
            if (i < _mainInventory.items.Count)
            {
                InventoryItem itemData = _mainInventory.items[i];
                if (itemData.id != 0)
                {
                    itemIcon.sprite = _iitem.items[itemData.id].image;
                    itemIcon.color = Color.white;
                }
                else
                {
                    itemIcon.sprite = null;
                    itemIcon.color = new Color(1, 1, 1, 0);
                }
            }
            else
            {
                itemIcon.sprite = null;
                itemIcon.color = new Color(1, 1, 1, 0);
            }

            // Включаем/отключаем компонент Image у слота в зависимости от выбора
            if (slotBackground != null)
            {
                slotBackground.enabled = (_slotIndex == i);
            }
        }
    }

    private void SwitchQuickAccess()
    {
        for (int i = 0; i < quickAccessCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _slotIndex = i;
                //ColorSlot();
            }
        }
    }

    //private void ColorSlot()
    //{
    //    for (int i = 0; i < quickAccessCount; i++)
    //    {
    //        // Подсветка должна менять фон слота, а не иконку!
    //        UnityEngine.UI.Image slotBackground = slotObject[i].GetComponent<UnityEngine.UI.Image>();

    //        if (slotBackground != null)
    //        {
    //            slotBackground.color = (_slotIndex == i)
    //                ? Color.green
    //                : new Color(1f, 0.878f, 0.667f);
    //        }
    //    }
    //}

    private void AnimationTool()
    {
        InventoryItem invItem = _mainInventory.items[_slotIndex];
        Item item = _iitem.items[invItem.id];
        _tool = item.toolType;
        animator.SetInteger(TOOLType, (int)_tool);
    }

    public void ClickControl()
    {
        if (Player.Instance.IsRunning())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (_slotIndex >= _mainInventory.items.Count || _mainInventory.items[_slotIndex].id == 0)
                return;

            InventoryItem invItem = _mainInventory.items[_slotIndex];
            Item currentItem = _iitem.items[invItem.id];

            if (currentItem.toolType == Item.ToolType.None)
                return;
            if (currentItem.toolType == Item.ToolType.Axe || currentItem.toolType == Item.ToolType.Pickaxe)
            {
                float attackWidth = 1.5f;
                float attackHeight = 2.2f;
                float attackOffset = 0.8f;
                Vector2 attackCenter = CheckPositionAndFlip().origin + (CheckPositionAndFlip().direction * attackOffset);
                Vector2 attackSize = new Vector2(attackWidth, attackHeight);
                debugAttackCenter = attackCenter;
                debugAttackSize = attackSize;

                Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                    attackCenter,
                    attackSize,
                    0f,
                    hitLayer
                );

                foreach (Collider2D col in hitColliders)
                {
                    DestructibleObject obj = col.GetComponent<DestructibleObject>();
                    if (obj != null)
                    {
                        obj.TakeDamage(currentItem.damage, currentItem.toolType);
                        GameObject effect = Instantiate(_effectPrefab, transform.position, Quaternion.identity);
                    }
                }

            }
            if (currentItem.toolType == Item.ToolType.Weapon)
            {
                float attackWidth = 1.5f;
                float attackHeight = 2.2f;
                float attackOffset = 0.8f;
                Vector2 attackCenter = CheckPositionAndFlip().origin + (CheckPositionAndFlip().direction * attackOffset);
                Vector2 attackSize = new Vector2(attackWidth, attackHeight);
                debugAttackCenter = attackCenter;
                debugAttackSize = attackSize;

                Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                    attackCenter,
                    attackSize,
                    0f,
                    hitLayerEnemy
                );

                foreach (Collider2D col in hitColliders)
                {
                    Enemy enemy = col.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(currentItem.damage);
                        GameObject effect = Instantiate(_effectPrefab, transform.position, Quaternion.identity);
                    }
                }

            }

            if (currentItem.toolType == Item.ToolType.Hoe)
            {
                float hoeRange = 1f;

                Vector2 hoeCeanter = CheckPositionAndFlip().origin + (CheckPositionAndFlip().direction * hoeRange);
                Vector3Int tilePosition = worldTilemap.WorldToCell(hoeCeanter);

                TileBase currentTile = worldTilemap.GetTile(tilePosition);
               
                if (currentTile != wateredTile)
                    waterManager.LandTile(tilePosition);
            }

            if(currentItem.toolType == Item.ToolType.WateringCan)
            {
                float waterRange = 1f;
                Vector2 waterCeanter = CheckPositionAndFlip().origin + (CheckPositionAndFlip().direction * waterRange);
                Vector3Int tilePosition = worldTilemap.WorldToCell(waterCeanter);
                TileBase currentTile = worldTilemap.GetTile(tilePosition);
               
                if (currentTile == tilledSoilTile)
                    waterManager.WaterTile(tilePosition);
            }

            if (currentItem.toolType == Item.ToolType.Seeds)
            {
                float waterRange = 1f;
                Vector2 groupCeanter = CheckPositionAndFlip().origin + CheckPositionAndFlip().direction* waterRange;
                Vector3Int tilePosition = cropsTilemap.WorldToCell(groupCeanter);
                TileBase currentTile = worldTilemap.GetTile(tilePosition);
                TileBase existingCrop = cropsTilemap.GetTile(tilePosition);

                CropData selectedCrop = cropManager.GetCropBySeedID(_mainInventory.items[_slotIndex].id);
                if (selectedCrop != null && existingCrop == null && (currentTile == tilledSoilTile || currentTile == wateredTile))
                {
                    cropsTilemap.SetTile(tilePosition, selectedCrop.growthStages[0]);
                    cropManager.PlantCrop(tilePosition, selectedCrop);

                    _mainInventory.items[_slotIndex].count--;
                    
                }

            }
            
            if (currentItem.toolType == Item.ToolType.Serp)
            {
                float harvestRange = 1f;
                Vector2 hitPoint = CheckPositionAndFlip().origin + (CheckPositionAndFlip().direction * harvestRange);

                Vector3Int tilePos = cropsTilemap.WorldToCell(hitPoint);

                TileBase cropTile = cropsTilemap.GetTile(tilePos);

                PlantedCrop crop = cropManager.GetCropAt(tilePos);

                if (crop != null && crop.currentStage == crop.cropData.growthStages.Length - 1)
                {

                    // Добавляем урожай в инвентарь
                    _mainInventory.SearchForSameItem(_iitem.items[crop.cropData.harvestItemID], crop.cropData.harvestCount);

                    // Удаляем растение
                    cropsTilemap.SetTile(tilePos, null);
                    cropManager.RemoveCrop(tilePos);

                }
               
            }
            AnimationTool();
            animator.SetBool(UseTool, true);

        }
        
    }

    public (Vector2 origin,Vector2 direction) CheckPositionAndFlip()
    {
        Vector2 origin = player.position;
        Vector2 direction = Player.Instance.LastMoveDirection;
        return (origin, direction);
    }




}