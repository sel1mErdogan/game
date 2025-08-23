using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Görsel Elemanlar")]
    [SerializeField] private GameObject fullSlotView;
    [SerializeField] private GameObject emptySlotView;
    [SerializeField] private TextMeshProUGUI saveNameText;
    [SerializeField] private TextMeshProUGUI lastSavedText;
    [SerializeField] private RawImage screenshotImage; // Ekran görüntüsü için
    [SerializeField] private Button loadButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button deleteButton; 

    private int slotIndex;
    private SaveLoadMenu parentMenu;
    private ConfirmationPanelUI confirmationPanel;

    private void Awake()
    {
        loadButton.onClick.AddListener(OnLoadClicked);
        saveButton.onClick.AddListener(OnSaveClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        deleteButton.onClick.AddListener(OnDeleteClicked);

        confirmationPanel = FindObjectOfType<ConfirmationPanelUI>(true); // Pasif olanı da bul
    }

    public void Setup(GameData data, int index, SaveLoadMenu menu)
    {
        slotIndex = index;
        parentMenu = menu;
        bool isSlotFull = data != null;
        
        fullSlotView.SetActive(isSlotFull);
        emptySlotView.SetActive(!isSlotFull);
        deleteButton.gameObject.SetActive(isSlotFull);

        if (isSlotFull)
        {
            saveNameText.text = data.saveName;
            lastSavedText.text = data.lastSaved;
            
            Texture2D screenshot = SaveSystem.LoadScreenshot(index);
            if (screenshot != null)
            {
                screenshotImage.texture = screenshot;
                screenshotImage.color = Color.white;
            }
            else
            {
                screenshotImage.color = Color.clear; // Resim yoksa görünmez yap
            }
        }
        else
        {
            screenshotImage.color = Color.clear;
        }
        
        bool isInGameScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "OyunSahnesi";
        saveButton.interactable = isInGameScene && isSlotFull;
    }

    private void OnLoadClicked() => GameManager.Instance.LoadGame(slotIndex);
    private void OnSaveClicked() 
    {
        GameManager.Instance.SaveGame(slotIndex); // Sadece kaydetme komutu verilir.
        parentMenu.PopulateSlots(); // Hemen menüyü yenilemeye çalışır (bu yüzden gecikme olur).
    }    
    private void OnNewGameClicked() => GameManager.Instance.StartNewGame(slotIndex);
    
    private void OnDeleteClicked()
    {
        confirmationPanel.Show($"'{saveNameText.text}' kaydını silmek istediğine emin misin?", () => {
            SaveSystem.Delete(slotIndex);
            parentMenu.PopulateSlots();
        });
    }
}