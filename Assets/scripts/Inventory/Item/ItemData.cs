using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        // İsterseniz buraya eşya açıklaması gibi ek bilgiler ekleyebilirsiniz.
        // public string description;
    }
}