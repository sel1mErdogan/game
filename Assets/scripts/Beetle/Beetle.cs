using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

namespace KingdomBug
{
    public class Beetle : MonoBehaviour
    {
        [SerializeField] private BeetleType beetleType;
        [SerializeField] private float collectRadius = 2f;
        [SerializeField] private int maxInventorySize = 5;
        
        private Dictionary<ItemData, int> beetleInventory = new Dictionary<ItemData, int>();
        
        private void Awake()
        {
            // Böceğin toplama yapabilmesi için bir trigger collider'a ihtiyacı var.
            var triggerCollider = gameObject.AddComponent<SphereCollider>();
            triggerCollider.radius = collectRadius;
            triggerCollider.isTrigger = true;
        }
        
        // Bu fonksiyon, böcek bir item'ın üzerine geldiğinde otomatik çalışır.
        private void OnTriggerEnter(Collider other)
        {
            WorldItem worldItem = other.GetComponent<WorldItem>();
            
            if (worldItem != null && CanCollectItem(worldItem.GetItemData()) && !IsInventoryFull())
            {
                ItemData itemData = worldItem.GetItemData();
                int quantity = worldItem.GetQuantity();
                
                if (beetleInventory.ContainsKey(itemData))
                {
                    beetleInventory[itemData] += quantity;
                }
                else
                {
                    beetleInventory.Add(itemData, quantity);
                }
                
                // Item toplandıktan sonra dünyadan silinir.
                worldItem.Collect();
            }
        }
        
        public bool CanCollectItem(ItemData itemData)
        {
            // itemData'nın null olma ihtimaline karşı kontrol.
            if (itemData == null) return false;

            ResourceType resourceType = itemData.resourceType;
   
            switch (beetleType)
            {
                case BeetleType.Worker:
                    // İşçi böcek SADECE yapı malzemelerini toplayabilir (kırıntı ve su HARİÇ)
                    return resourceType != ResourceType.Crumb && resourceType != ResourceType.Water;
       
                case BeetleType.Explorer:
                    // Keşifçi böcek SADECE kırıntı ve su toplayabilir
                    return resourceType == ResourceType.Crumb || resourceType == ResourceType.Water;
       
                case BeetleType.Master:
                case BeetleType.Warrior:
                    return false;
       
                default:
                    return false;
            }
        }

        public BeetleType GetBeetleType()
        {
            return beetleType;
        }
        
        public Dictionary<ItemData, int> EmptyInventory()
        {
            Dictionary<ItemData, int> items = new Dictionary<ItemData, int>(beetleInventory);
            beetleInventory.Clear();
            return items;
        }
        
        public bool HasItems()
        {
            return GetTotalItemCount() > 0;
        }

        private int GetTotalItemCount()
        {
            int total = 0;
            foreach (var count in beetleInventory.Values)
            {
                total += count;
            }
            return total;
        }

        public bool IsInventoryFull()
        {
            return GetTotalItemCount() >= maxInventorySize;
        }
        
        private void OnDrawGizmosSelected()
        {
            // Bu sadece editörde görünen bir görseldir, debug mesajı değildir.
            // Böceğin toplama alanını (OnTriggerEnter) gösterir.
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Turuncu
            Gizmos.DrawSphere(transform.position, collectRadius);
        }
    }
}