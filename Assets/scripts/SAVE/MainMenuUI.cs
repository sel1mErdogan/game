using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject settingsPanel;

    private const string LastUsedSlotKey = "LastUsedSlot";

    private void Start()
    {
        BackToMainMenu();
    }

    // "Play" butonu artık bu yeni mantıkla çalışacak
    public void OnPlayButton()
    {
        // 1. "Devam Et" seçeneği var mı?
        if (PlayerPrefs.HasKey(LastUsedSlotKey))
        {
            int lastSlot = PlayerPrefs.GetInt(LastUsedSlotKey);
            if (SaveSystem.SaveFileExists(lastSlot))
            {
                // Evet, var. Direkt o oyuna devam et.
                GameManager.Instance.ContinueGame(lastSlot);
                return;
            }
        }
        
        // 2. "Devam Et" yoksa, HİÇ kayıtlı oyun var mı?
        if (SaveSystem.DoesAnySaveFileExist())
        {
            // Evet, başka kayıtlar var. Oyuncunun seçmesi için menüyü aç.
            OpenSaveLoadMenu();
        }
        else
        {
            // Hayır, HİÇ kayıt yok (ilk oynanış). Direkt yeni oyun başlat.
            // Otomatik olarak 0 numaralı yuvaya yeni bir krallık kur.
            GameManager.Instance.StartNewGame(0);
        }
    }

    // "Save" butonu bu fonksiyonu çağıracak
    public void OpenSaveLoadMenu()
    {
        mainMenuPanel.SetActive(false);
        saveLoadPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        mainMenuPanel.SetActive(false);
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
}