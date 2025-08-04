// InventoryManager.cs - Değişiklikler
using System.Collections.Generic;
using UnityEngine;
using System;

namespace InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        // Singleton Pattern
        public static InventoryManager Instance { get; private set; }

        // Farklı envanter türleri için dictionary
        private Dictionary<string, Dictionary<ItemData, int>> inventories = new Dictionary<string, Dictionary<ItemData, int>>();
        
        // Aktif envanter
        [SerializeField] private string activeInventory = "Colony Stockpile";
        
        [SerializeField] private int maxCapacity = 20;

        // Envanter değiştiğinde UI'ı güncellemek için bir event
        public event Action OnInventoryChanged;

        private void Awake()
        {
            // Singleton kurulumu
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            
            // Varsayılan envanterleri oluştur
            inventories["Colony Stockpile"] = new Dictionary<ItemData, int>();
            inventories["Player Backpack"] = new Dictionary<ItemData, int>();
        }

        /// <summary>
        /// Belirtilen envantere yeni bir eşya ekler.
        /// </summary>
        public bool AddItem(ItemData item, int quantity, string inventoryName = null)
        {
            // Eğer envanter adı belirtilmemişse, aktif envanteri kullan
            if (string.IsNullOrEmpty(inventoryName))
            {
                inventoryName = activeInventory;
            }
            
            // Belirtilen envanter var mı kontrol et
            if (!inventories.ContainsKey(inventoryName))
            {
                Debug.LogError($"'{inventoryName}' adında bir envanter bulunamadı!");
                return false;
            }
            
            var inventory = inventories[inventoryName];
            
            // Envanter kapasitesi kontrolü
            int currentTotalItems = GetTotalItemCount(inventoryName);
            if (currentTotalItems + quantity > maxCapacity)
            {
                Debug.LogWarning($"'{inventoryName}' envanteri dolu! Eşya eklenemedi.");
                return false;
            }

            // Eşya envanterde zaten varsa, sayısını artır
            if (inventory.ContainsKey(item))
            {
                inventory[item] += quantity;
            }
            // Yoksa, envantere yeni bir giriş olarak ekle
            else
            {
                inventory.Add(item, quantity);
            }

            Debug.Log($"{quantity} adet {item.itemName}, '{inventoryName}' envanterine eklendi.");
            
            // UI'ın güncellenmesi için event'i tetikle
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Belirtilen envanterdeki tüm eşyaları ve adetlerini döner.
        /// </summary>
        public Dictionary<ItemData, int> GetInventory(string inventoryName = null)
        {
            // Eğer envanter adı belirtilmemişse, aktif envanteri kullan
            if (string.IsNullOrEmpty(inventoryName))
            {
                inventoryName = activeInventory;
            }
            
            // Belirtilen envanter var mı kontrol et
            if (!inventories.ContainsKey(inventoryName))
            {
                Debug.LogError($"'{inventoryName}' adında bir envanter bulunamadı!");
                return new Dictionary<ItemData, int>();
            }
            
            return inventories[inventoryName];
        }

        /// <summary>
        /// Aktif envanteri değiştirir.
        /// </summary>
        public void SetActiveInventory(string inventoryName)
        {
            if (inventories.ContainsKey(inventoryName))
            {
                activeInventory = inventoryName;
                OnInventoryChanged?.Invoke();
            }
            else
            {
                Debug.LogError($"'{inventoryName}' adında bir envanter bulunamadı!");
            }
        }

        /// <summary>
        /// Belirtilen envanterdeki toplam eşya sayısını hesaplar.
        /// </summary>
        private int GetTotalItemCount(string inventoryName = null)
        {
            // Eğer envanter adı belirtilmemişse, aktif envanteri kullan
            if (string.IsNullOrEmpty(inventoryName))
            {
                inventoryName = activeInventory;
            }
            
            // Belirtilen envanter var mı kontrol et
            if (!inventories.ContainsKey(inventoryName))
            {
                Debug.LogError($"'{inventoryName}' adında bir envanter bulunamadı!");
                return 0;
            }
            
            var inventory = inventories[inventoryName];
            int total = 0;
            
            foreach (var count in inventory.Values)
            {
                total += count;
            }
            
            return total;
        }
    }
}
