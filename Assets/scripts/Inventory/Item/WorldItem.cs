using UnityEngine;
using InventorySystem;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;
    
    public ItemData GetItemData()
    {
        return itemData;
    }
    
    public int GetQuantity()
    {
        return quantity;
    }
    
    // Item alındığında yok et
    public void Collect()
    {
        Destroy(gameObject);
    }
}