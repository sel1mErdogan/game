// InventoryUI.cs
using UnityEngine;
using TMPro; // TextMeshPro kullanacağız, daha esnek.
using System.Text;

namespace InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        // Ekranda envanteri göstereceğimiz TextMeshPro objesi.
        [SerializeField] private TextMeshProUGUI inventoryText;
    
        private void OnEnable()
        {
            // InventoryManager'daki event'e abone ol.
            InventoryManager.Instance.OnInventoryChanged += UpdateUI;
        }
    
        private void OnDisable()
        {
            // Unutma! Obje kapandığında veya yok olduğunda event'ten aboneliği kaldır.
            InventoryManager.Instance.OnInventoryChanged -= UpdateUI;
        }
    
        private void Start()
        {
            // Oyun başında arayüzü bir kere güncelle.
            UpdateUI();
        }
    
        /// <summary>
        /// InventoryManager'dan aldığı verilerle UI'ı günceller.
        /// </summary>
        private void UpdateUI()
        {
            if (inventoryText == null) return;
    
            var inventory = InventoryManager.Instance.GetInventory();
            StringBuilder sb = new StringBuilder("Envanter:\n");
    
            if (inventory.Count == 0)
            {
                sb.Append("Boş");
            }
            else
            {
                foreach (var itemEntry in inventory)
                {
                    // Örnek: "Kürdan: 5"
                    sb.AppendLine($"{itemEntry.Key.itemName}: {itemEntry.Value}");
                }
            }
            
            inventoryText.text = sb.ToString();
        }
    }
}
