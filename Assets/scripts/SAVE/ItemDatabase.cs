using UnityEngine;
using System.Collections.Generic;
using InventorySystem; // ItemData'yı tanımak için

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Tooltip("Projedeki TÜM ItemData ScriptableObject'larını buraya sürükleyin.")]
    [SerializeField] private List<ItemData> allItems;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Verilen isme göre ItemData'yı listeden bulup döndürür. Bu bizim telefon rehberi fonksiyonumuz.
    /// </summary>
    public ItemData FindItemByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return allItems.Find(item => item.itemName == name);
    }
}