using System.Collections.Generic;
using UnityEngine;
using System;

namespace InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        private Dictionary<ItemData, int> colonyStockpile = new Dictionary<ItemData, int>();
        public event Action OnInventoryChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        // Böceklerin veya oyuncunun envantere kaynak eklemesi için.
        public void AddItem(ItemData item, int quantity)
        {
            if (item == null || quantity <= 0) return;

            if (colonyStockpile.ContainsKey(item))
            {
                colonyStockpile[item] += quantity;
            }
            else
            {
                colonyStockpile.Add(item, quantity);
            }
            OnInventoryChanged?.Invoke();
        }

        // YENİ EKLENDİ: İnşaat iptal edildiğinde kaynakları geri iade etmek için.
        public void AddResources(List<ResourceCost> costs)
        {
            if (costs == null) return;

            foreach (var cost in costs)
            {
                AddItem(cost.resource, cost.amount);
            }
        }

        // Bir yapı inşa etmek için kaynakları harcar.
        public bool SpendResources(List<ResourceCost> costs)
        {
            // 1. Adım: Önce kaynakların yeterli olup olmadığını kontrol et.
            foreach (var cost in costs)
            {
                if (!HasEnough(cost.resource, cost.amount))
                {
                    Debug.LogWarning($"Yetersiz kaynak: {cost.amount} adet {cost.resource.itemName} gerekli.");
                    return false; // Yeterli kaynak yok, işlemi baştan iptal et.
                }
            }

            // 2. Adım: Tüm kaynaklar yeterliyse, şimdi hepsini harca.
            foreach (var cost in costs)
            {
                colonyStockpile[cost.resource] -= cost.amount;
                // Eğer bir kaynağın sayısı sıfıra düşerse, envanter listesinden kaldır.
                if (colonyStockpile[cost.resource] <= 0)
                {
                    colonyStockpile.Remove(cost.resource);
                }
            }

            OnInventoryChanged?.Invoke();
            return true; // İşlem başarılı.
        }

        // Belirli bir kaynaktan yeterli miktarda olup olmadığını kontrol eder.
        private bool HasEnough(ItemData resource, int amount)
        {
            if (colonyStockpile.TryGetValue(resource, out int currentAmount))
            {
                return currentAmount >= amount;
            }
            return false; // Envanterde o kaynaktan hiç yok.
        }
        
        // UI'ın envanteri okuması için.
        public Dictionary<ItemData, int> GetColonyStockpile()
        {
            return colonyStockpile;
        }
    }
}