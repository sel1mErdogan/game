using UnityEngine;
using InventorySystem;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private float collectRadius = 2f;
    [SerializeField] private LayerMask itemLayer;
    
    private void Awake()
    {
        var triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.radius = collectRadius;
        triggerCollider.isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        WorldItem worldItem = other.GetComponent<WorldItem>();
        
        if (worldItem != null)
        {
            ItemData itemData = worldItem.GetItemData();
            int quantity = worldItem.GetQuantity();
            
            // DÜZELTME: AddItem artık bool döndürmediği için "if" koşulu kaldırıldı.
            InventoryManager.Instance.AddItem(itemData, quantity);
            
            // Item eklendikten sonra dünya nesnesini yok et.
            worldItem.Collect();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}