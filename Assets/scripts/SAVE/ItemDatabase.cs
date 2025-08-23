// --- ItemDatabase.cs ---
using UnityEngine;
using System.Collections.Generic;
using InventorySystem; // ItemData sınıfına erişim için

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Tooltip("Projedeki TÜM ItemData ScriptableObject'larını buraya sürükleyin.")]
    [SerializeField] private List<ItemData> allItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Verilen isme göre ItemData'yı listeden bulup döndürür.
    public ItemData FindItemByName(string name)
    {
        return allItems.Find(item => item.itemName == name);
    }
}