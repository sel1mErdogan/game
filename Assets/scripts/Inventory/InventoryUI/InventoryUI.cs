using UnityEngine;
using TMPro;
using System.Text;

namespace InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI inventoryText;
    
        private void OnEnable()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged += UpdateUI;
        }
    
        private void OnDisable()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged -= UpdateUI;
        }
    
        private void Start()
        {
            UpdateUI();
        }
    
        private void UpdateUI()
        {
            if (inventoryText == null) return;
    
            // DÜZELTME: Fonksiyon adı GetColonyStockpile olarak değiştirildi.
            var inventory = InventoryManager.Instance.GetColonyStockpile();
            StringBuilder sb = new StringBuilder("Koloni Deposu:\n");
    
            if (inventory.Count == 0)
            {
                sb.Append("Boş");
            }
            else
            {
                foreach (var itemEntry in inventory)
                {
                    sb.AppendLine($"{itemEntry.Key.itemName}: {itemEntry.Value}");
                }
            }
            
            inventoryText.text = sb.ToString();
        }
    }
}