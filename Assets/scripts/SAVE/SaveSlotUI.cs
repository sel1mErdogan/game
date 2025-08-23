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
    [SerializeField] private Button loadButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button deleteButton; 

    private int slotIndex;
    private SaveLoadMenu parentMenu;

    private void Awake()
    {
        loadButton.onClick.AddListener(OnLoadClicked);
        saveButton.onClick.AddListener(OnSaveClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        deleteButton.onClick.AddListener(OnDeleteClicked);
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
        }
        
        // Ana Menü'de isek, yani oyun sahnesi açık değilse, "Kaydet" butonu tıklanamaz olsun.
        bool isInGameScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "OyunSahnesi";
        saveButton.interactable = isInGameScene && isSlotFull;
    }

    private void OnLoadClicked() => GameManager.Instance.LoadGame(slotIndex);
    
    private void OnSaveClicked() 
    {
        GameManager.Instance.SaveGame(slotIndex);
        parentMenu.PopulateSlots(); // Tarih gibi bilgileri güncellemek için menüyü yenile
    }
    
    private void OnNewGameClicked() => GameManager.Instance.StartNewGame(slotIndex);
    
    private void OnDeleteClicked()
    {
        SaveSystem.Delete(slotIndex);
        parentMenu.PopulateSlots(); // Silindikten sonra menüyü yenile
    }
}