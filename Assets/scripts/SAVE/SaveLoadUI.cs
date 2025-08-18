using UnityEngine;
using System.Collections.Generic;

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private Transform slotsContainer; // Kayıt yuvalarının oluşturulacağı yer
    [SerializeField] private GameObject saveSlotPrefab; // Tek bir kayıt yuvası prefab'ı

    // Bu panel her açıldığında, yuvaları güncel bilgilerle doldurur
    private void OnEnable()
    {
        PopulateSaveSlots();
    }

    public void PopulateSaveSlots()
    {
        // Önce eski yuvaları temizle
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }

        // Tüm kayıt yuvalarındaki verileri sistemden al
        List<GameData> allSaveData = SaveSystem.GetAllSaveData();

        for (int i = 0; i < allSaveData.Count; i++)
        {
            GameObject slotInstance = Instantiate(saveSlotPrefab, slotsContainer);
            SaveSlotUI slotUI = slotInstance.GetComponent<SaveSlotUI>();
            if (slotUI != null)
            {
                // Her bir yuvaya kendi bilgilerini ve ne yapacağını söyle
                slotUI.Setup(i, allSaveData[i], this);
            }
        }
    }
}