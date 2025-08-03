// Beetle.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

namespace KingdomBug
{
    public class Beetle : MonoBehaviour
    {
        [SerializeField] private BeetleType beetleType;
        [SerializeField] private float collectRadius = 2f;
        [SerializeField] private LayerMask itemLayer;
        [SerializeField] private int maxInventorySize = 5;
        
        private SphereCollider triggerCollider;
        private Dictionary<ItemData, int> beetleInventory = new Dictionary<ItemData, int>();
        
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
                // Böcek türüne göre toplama kontrolü
                if (CanCollectItem(worldItem.GetItemData()))
                {
                    // Böceğin envanteri dolu mu kontrol et
                    if (GetTotalItemCount() >= maxInventorySize)
                    {
                        Debug.Log($"{beetleType} böceğinin envanteri dolu!");
                        return;
                    }
                    
                    // Item'ı böceğin envanterine ekle
                    ItemData itemData = worldItem.GetItemData();
                    int quantity = worldItem.GetQuantity();
                    
                    // Aynı türden item varsa miktarını artır, yoksa yeni ekle
                    if (beetleInventory.ContainsKey(itemData))
                    {
                        beetleInventory[itemData] += quantity;
                    }
                    else
                    {
                        beetleInventory.Add(itemData, quantity);
                    }
                    
                    // Dünya nesnesini yok et
                    worldItem.Collect();
                    Debug.Log($"{beetleType} böceği {itemData.itemName} topladı.");
                }
                else
                {
                    Debug.Log($"{beetleType} böceği {worldItem.GetItemData().itemName} toplayamaz!");
                }
            }
        }
        
        // Böcek türüne göre toplayabilme kontrolü - Düzeltilmiş versiyon
        public bool CanCollectItem(ItemData itemData)
        {
            ResourceType resourceType = itemData.resourceType;
    
            Debug.Log($"Kontrol: {beetleType} böceği {itemData.itemName} (gerçek tür: {resourceType}) toplayabilir mi?");
    
            switch (beetleType)
            {
                case BeetleType.Worker:
                    // İşçi böcek SADECE yapı malzemelerini toplayabilir (kırıntı ve su HARİÇ)
                    bool canWorkerCollect = resourceType != ResourceType.Crumb && 
                                            resourceType != ResourceType.Water;
                    Debug.Log($"İşçi böcek toplayabilir mi: {canWorkerCollect}");
                    return canWorkerCollect;
        
                case BeetleType.Explorer:
                    // Keşifçi böcek SADECE kırıntı ve su toplayabilir
                    bool canExplorerCollect = resourceType == ResourceType.Crumb || 
                                              resourceType == ResourceType.Water;
                    Debug.Log($"Keşifçi böcek toplayabilir mi: {canExplorerCollect}");
                    return canExplorerCollect;
            
                case BeetleType.Master:
                    Debug.Log("Usta böcek hiçbir şey toplamaz");
                    return false;
            
                case BeetleType.Warrior:
                    Debug.Log("Savaşçı böcek hiçbir şey toplamaz");
                    return false;
            
                default:
                    Debug.Log("Bilinmeyen böcek türü");
                    return false;
            }
        }



        
        // Böcek üsse döndüğünde envanterini boşaltma
        public Dictionary<ItemData, int> EmptyInventory()
        {
            Dictionary<ItemData, int> items = new Dictionary<ItemData, int>(beetleInventory);
            beetleInventory.Clear();
            Debug.Log($"{beetleType} böceği envanterini boşalttı.");
            return items;
        }
        
        // Böceğin envanterinde item var mı kontrol
        public bool HasItems()
        {
            return beetleInventory.Count > 0;
        }
        public BeetleType GetBeetleType()
        {
            return beetleType;
        }
        // Envanterdeki toplam item sayısını hesapla
        private int GetTotalItemCount()
        {
            int total = 0;
            foreach (var count in beetleInventory.Values)
            {
                total += count;
            }
            return total;
        }
        
        // Toplama mesafesini görselleştirmek için (sadece editor'da)
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, collectRadius);
        }
    }
}
