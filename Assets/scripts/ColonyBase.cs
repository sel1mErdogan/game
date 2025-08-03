// ColonyBase.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using KingdomBug;

public class ColonyBase : MonoBehaviour
{
    [SerializeField] private float depositRadius = 5f;
    [SerializeField] private string colonyInventoryName = "Colony Stockpile";
    
    private SphereCollider triggerCollider;
    
    private void Awake()
    {
        // Trigger collider oluştur
        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.radius = depositRadius;
        triggerCollider.isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Giren nesne bir böcek mi kontrol et
        Beetle beetle = other.GetComponent<Beetle>();
        
        if (beetle != null && beetle.HasItems())
        {
            // Böceğin envanterindeki itemları al
            Dictionary<ItemData, int> items = beetle.EmptyInventory();
            
            // Itemları koloni envanterine ekle
            foreach (var item in items)
            {
                InventoryManager.Instance.AddItem(item.Key, item.Value);
                Debug.Log($"Koloni envanterine {item.Value} adet {item.Key.itemName} eklendi.");
            }
        }
    }
    
    // Üs bölgesini görselleştirmek için (sadece editor'da)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, depositRadius);
    }
}