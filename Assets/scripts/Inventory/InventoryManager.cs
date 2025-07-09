// InventoryManager.cs
using System.Collections.Generic;
using UnityEngine;
using System; // Event'ler için gerekli

namespace InventorySystem
{
    public class InventoryManager : MonoBehaviour
{
    // Singleton Pattern: Bu script'e her yerden kolayca erişmemizi sağlar.
    public static InventoryManager Instance { get; private set; }

    // Envanterdeki eşyaları ve adetlerini tutan dictionary.
    private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    [SerializeField] private int maxCapacity = 20; // GDD'deki gibi envanter kapasitesi. [cite: 97]

    // Envanter değiştiğinde UI'ı güncellemek için bir event.
    public event Action OnInventoryChanged;
    [SerializeField] private InventoryUI inventoryUI;
        
    private Dictionary<string, List<ItemData>> inventories = new Dictionary<string, List<ItemData>>();
    private void Start()
    {
        // Örnek olarak bir envanter oluşturalım
        inventories["Colony Stockpile"] = new List<ItemData>();
        inventories["Player Backpack"] = new List<ItemData>();
    }

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
            // Sahne değiştğinde bu objenin silinmemesini sağlar (opsiyonel ama tavsiye edilir).
            // DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Envantere yeni bir eşya ekler.
    /// </summary>
    /// <param name="item">Eklenecek eşyanın ItemData'sı</param>
    /// <param name="quantity">Eklenecek adet</param>
    /// <returns>Ekleme başarılıysa true döner</returns>
    public bool AddItem(ItemData item, int quantity)
    {
        int currentTotalItems = GetTotalItemCount();
        if (currentTotalItems + quantity > maxCapacity)
        {
            Debug.LogWarning("Envanter dolu! Eşya eklenemedi.");
            return false;
        }

        // Eğer eşya envanterde zaten varsa, sayısını artır.
        if (inventory.ContainsKey(item))
        {
            inventory[item] += quantity;
        }
        // Yoksa, envantere yeni bir giriş olarak ekle.
        else
        {
            inventory.Add(item, quantity);
        }

        Debug.Log($"{quantity} adet {item.itemName} envantere eklendi.");
        // UI'ın güncellenmesi için event'i tetikle.
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Envanterdeki tüm eşyaları ve adetlerini döner.
    /// </summary>
    public Dictionary<ItemData, int> GetInventory()
    {
        return inventory;
    }

    /// <summary>
    /// Envanterdeki toplam eşya sayısını hesaplar.
    /// </summary>
    private int GetTotalItemCount()
    {
        int total = 0;
        foreach (var count in inventory.Values)
        {
            total += count;
        }
        return total;
    }
}
}
