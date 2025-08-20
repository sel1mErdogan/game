using UnityEngine;
using InventorySystem;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;
    [SerializeField] private string id;

    [ContextMenu("Generate GUID")]
    private void GenerateGUID() => id = System.Guid.NewGuid().ToString();
    
    public ItemData GetItemData() => itemData;
    public int GetQuantity() => quantity;
    public string GetID() => id;
    
    public void Collect()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(id))
        {
            GameManager.Instance.NotifyItemCollected(id);
        }
        Destroy(gameObject);
    }
}