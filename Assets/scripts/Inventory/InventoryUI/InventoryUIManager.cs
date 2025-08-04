// InventoryUIManager.cs
using System.Collections;
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
    [SerializeField] private int gridColumns = 3;
    [SerializeField] private int gridRows = 3;

    private List<ItemSlotUI> itemSlots = new List<ItemSlotUI>();
    private bool isInventoryOpen = false;

    private void Start()
    {
        // Envanter panelini başlangıçta kapat
        inventoryPanel.SetActive(false);
        
        // Envanter butonuna tıklama olayını ekle
        inventoryButton.onClick.AddListener(ToggleInventory);
        
        // Envanter başlığını ayarla
        inventoryTitleText.text = inventoryTitle;
        
        // Slot'ları oluştur
        CreateItemSlots();
        
        // Envanter değişim olayına abone ol
        InventoryManager.Instance.OnInventoryChanged += UpdateInventoryUI;
    }
    
    private void OnDestroy()
    {
        // Olay aboneliğini kaldır
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= UpdateInventoryUI;
    }
    
    // Envanter panelini açıp kapatma
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }
    
    // Grid için slot'ları oluştur
    private void CreateItemSlots()
    {
        // Önce varsa eski slotları temizle
        foreach (Transform child in itemsGrid)
        {
            Destroy(child.gameObject);
        }
        itemSlots.Clear();
        
        // Yeni slotları oluştur
        for (int i = 0; i < gridRows * gridColumns; i++)
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
    
    // Envanter içeriğini UI'a yansıt
    private void UpdateInventoryUI()
    {
        if (!isInventoryOpen) return;
        
        // Tüm slotları temizle
        foreach (var slot in itemSlots)
        {
            slot.ClearSlot();
        }
        
        // Envanter içeriğini al
        var inventory = InventoryManager.Instance.GetInventory();
        int slotIndex = 0;
        
        // Her eşyayı bir slota yerleştir
        foreach (var item in inventory)
        {
            if (slotIndex < itemSlots.Count)
            {
                itemSlots[slotIndex].SetItem(item.Key, item.Value);
                slotIndex++;
            }
            else
            {
                Debug.LogWarning("Envanterde gösterilecek yer kalmadı!");
                break;
            }
        }
    }
}
