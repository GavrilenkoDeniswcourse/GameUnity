using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Здоровье обьекта")]
    [SerializeField] private int _maxHealth = 3;
    [Header("Префаб ресурса выпадения")]
    [SerializeField] private GameObject _dropPrefab;
    [Header("Количество выпадения")]
     public int _minOre = 1;
    public int _maxOre = 3;
    //[SerializeField] private GameObject _effectPrefab;
   

    private int _currentHealth;
    private int _currentCount;

    // Опционально: какой тип инструмента нужен для разрушения
    [SerializeField] private Item.ToolType requiredTool = Item.ToolType.Axe;

    void Start()
    {
        _currentHealth = _maxHealth;
        
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        //GameObject effect = Instantiate(_effectPrefab, transform.position, Quaternion.identity);

        if (_currentHealth <= 0)
        {
            DropResource();
            Destroy(gameObject);
            
        }
    }

   
    public void TakeDamage(int damage, Item.ToolType toolType)
    {
        if (requiredTool != Item.ToolType.None && toolType != requiredTool)
        {
            return;
        }

        TakeDamage(damage);
    }

    private void DropResource()
    {
        int ore = Random.Range(_minOre, _maxOre + 1);

        if (_dropPrefab != null)
        {
            for (int i = 0; i < ore; i++)
            {
               // GameObject effect = Instantiate(_effectPrefab, transform.position, Quaternion.identity);
                Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
                Vector3 dropPos = transform.position + (Vector3)randomOffset;
                Instantiate(_dropPrefab, dropPos, Quaternion.identity);

            }
        }
    }
}