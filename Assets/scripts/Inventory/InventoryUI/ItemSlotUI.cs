using UnityEngine;
using UnityEngine.UI;
using InventorySystem;

public class ItemSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text quantityText;
    [SerializeField] private GameObject emptySlotOverlay; // Boş slot görseli

    private ItemData currentItem;
    private int currentQuantity;
    
    private void Awake()
    {
        // Başlangıçta slot boş olarak ayarla
        ClearSlot();
    }
    
    public void SetItem(ItemData item, int quantity)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }
        
        currentItem = item;
        currentQuantity = quantity;
        
        // Icon'u ayarla
        if (itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
            itemIcon.color = Color.white; // Görünür yap
        }
        
        // Miktar metnini ayarla
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
            quantityText.enabled = quantity > 1;
        }
        
        // Boş slot göstergesini kapat
        if (emptySlotOverlay != null)
        {
            emptySlotOverlay.SetActive(false);
        }
    }
    
    public void ClearSlot()
    {
        currentItem = null;
        currentQuantity = 0;
        
        // Icon'u temizle
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
        
        // Miktar metnini temizle
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }
        
        // Boş slot göstergesini aç
        if (emptySlotOverlay != null)
        {
            emptySlotOverlay.SetActive(true);
        }
    }
    
    // Eşya bilgilerini gösterme (tooltip)
    public void ShowItemTooltip()
    {
        if (currentItem != null)
        {
            // Tooltip sistemini burada ekleyebilirsiniz
            Debug.Log($"Eşya: {currentItem.itemName}\nAçıklama: {currentItem.description}");
        }
    }
    
    // Slot'taki eşyayı ve miktarını döndür
    public (ItemData item, int quantity) GetItemInfo()
    {
        return (currentItem, currentQuantity);
    }
    
    // Slot dolu mu kontrol et
    public bool IsEmpty()
    {
        return currentItem == null;
    }
}
