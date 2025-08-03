using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private float collectRadius = 2f; // Toplama mesafesi
    [SerializeField] private LayerMask itemLayer; // Item'ların layer'ı
    
    private SphereCollider triggerCollider;
    
    private void Awake()
    {
        // Trigger collider oluştur
        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.radius = collectRadius;
        triggerCollider.isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Giren nesne bir WorldItem mi kontrol et
        WorldItem worldItem = other.GetComponent<WorldItem>();
        
        if (worldItem != null)
        {
            // Item'ı envantere ekle
            ItemData itemData = worldItem.GetItemData();
            int quantity = worldItem.GetQuantity();
            
            if (InventoryManager.Instance.AddItem(itemData, quantity))
            {
                // Başarıyla eklendiyse, dünya nesnesini yok et
                worldItem.Collect();
                Debug.Log($"{itemData.itemName} otomatik olarak envantere eklendi.");
            }
        }
    }
    
    // Toplama mesafesini görselleştirmek için (sadece editor'da)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}