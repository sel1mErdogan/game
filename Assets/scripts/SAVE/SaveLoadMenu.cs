using UnityEngine;
using System.Collections.Generic;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private GameObject saveSlotPrefab;

    // Bu panel her açıldığında (aktif olduğunda) slotları günceller.
    private void OnEnable()
    {
        PopulateSlots();
    }

    public void PopulateSlots()
    {
        // Önce eskiden kalan slotları temizle
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }

        // Sistemden tüm kayıt dosyası bilgilerini çek
        List<GameData> allProfiles = SaveSystem.LoadAllSaveProfiles();

        for (int i = 0; i < SaveSystem.MAX_SAVE_SLOTS; i++)
        {
            // Yeni bir slot objesi yarat
            GameObject slotInstance = Instantiate(saveSlotPrefab, slotsContainer);
            SaveSlotUI slotUI = slotInstance.GetComponent<SaveSlotUI>();
            
            // Slota bilgilerini gönder ve kendini tanıt
            // Eğer o slot boşsa, allProfiles[i] null olacaktır. Bu bir sorun değil.
            slotUI.Setup(allProfiles[i], i, this);
        }
    }
}