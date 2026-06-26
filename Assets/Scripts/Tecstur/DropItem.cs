using UnityEngine;


public class DropItem : MonoBehaviour
{
   
    [SerializeField] private int itemID;
    private int amount = 1;

  
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            InventoryDisplay inventory = FindFirstObjectByType<InventoryDisplay>();
            if(inventory != null)
            {
                IItem itemBD = FindFirstObjectByType<IItem>();
                Item item = itemBD.items[itemID];

                inventory.SearchForSameItem(item, amount);

                Destroy(gameObject);
            }
            
            
        }
    }
}
