using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InventorySystem;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Transform itemsGrid;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Text inventoryTitleText;

    [Header("Settings")]
    [SerializeField] private string inventoryTitle = "Colony Stockpile";

    private List<ItemSlotUI> itemSlots = new List<ItemSlotUI>();
    private bool isInventoryOpen = false;

    private void Start()
    {
        inventoryPanel.SetActive(false);
        inventoryButton.onClick.AddListener(ToggleInventory);
        inventoryTitleText.text = inventoryTitle;
        CreateItemSlots();
        
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += UpdateInventoryUI;
    }
    
    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= UpdateInventoryUI;
    }
    
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }
    
    private void CreateItemSlots()
    {
        foreach (Transform child in itemsGrid)
        {
            Destroy(child.gameObject);
        }
        itemSlots.Clear();
        
        int slotCount = 20; // Bu değeri envanter kapasitesine göre ayarlayabilirsin
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemsGrid);
            ItemSlotUI slot = slotObj.GetComponent<ItemSlotUI>();
            if (slot != null)
            {
                slot.ClearSlot();
                itemSlots.Add(slot);
            }
        }
    }
    
    private void UpdateInventoryUI()
    {
        if (!isInventoryOpen) return;
        
        foreach (var slot in itemSlots)
        {
            slot.ClearSlot();
        }
        
        // DÜZELTME: Fonksiyon adı GetColonyStockpile olarak değiştirildi.
        var inventory = InventoryManager.Instance.GetColonyStockpile();
        int slotIndex = 0;
        
        foreach (var item in inventory)
        {
            if (slotIndex < itemSlots.Count)
            {
                itemSlots[slotIndex].SetItem(item.Key, item.Value);
                slotIndex++;
            }
            else
            {
                break;
            }
        }
    }
}