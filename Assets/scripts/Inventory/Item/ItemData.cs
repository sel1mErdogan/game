// ItemData.cs
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        public ResourceType resourceType; // Kaynak türünü ekledik
        [TextArea(3, 5)]
        public string description; // Açıklama ekledik
    }
}