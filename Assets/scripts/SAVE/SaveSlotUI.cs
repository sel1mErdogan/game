using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Sahne adını kontrol etmek için

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private GameObject emptySlotInfo;
    [SerializeField] private GameObject fullSlotInfo;
    [SerializeField] private TextMeshProUGUI saveNameText;
    [SerializeField] private TextMeshProUGUI lastSavedText;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    private int slotIndex;
    private SaveLoadUI parentUI;

    public void Setup(int index, GameData data, SaveLoadUI ui)
    {
        slotIndex = index;
        parentUI = ui;

        if (data == null)
        {
            emptySlotInfo.SetActive(true);
            fullSlotInfo.SetActive(false);
        }
        else
        {
            emptySlotInfo.SetActive(false);
            fullSlotInfo.SetActive(true);
            saveNameText.text = data.saveName;
            lastSavedText.text = data.lastSaved;
        }

        saveButton.onClick.AddListener(OnSaveClick);
        loadButton.onClick.AddListener(OnLoadClick);
        deleteButton.onClick.AddListener(OnDeleteClick);
    }

    public void OnSaveClick()
    {
        // --- YENİ MANTIK BURADA ---
        // Şu anki sahne Ana Menü mü?
        if (SceneManager.GetActiveScene().name == "AnaMenu")
        {
            // Evet, o zaman yeni oyun başlat.
            GameManager.Instance.StartNewGame(slotIndex);
        }
        else // Değilse, demek ki oyun sahnesindeyiz.
        {
            // O zaman mevcut oyunu bu yuvaya kaydet.
            GameManager.Instance.SaveCurrentGame(slotIndex);
            // Paneli yenile ki yeni kayıt görünsün.
            parentUI.PopulateSaveSlots();
        }
    }

    public void OnLoadClick()
    {
        GameManager.Instance.ContinueGame(slotIndex);
    }

    public void OnDeleteClick()
    {
        SaveSystem.DeleteSave(slotIndex);
        parentUI.PopulateSaveSlots();
    }
}