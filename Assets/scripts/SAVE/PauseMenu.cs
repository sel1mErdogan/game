using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool IsGamePaused = false;

    [Header("Panel Referansları")]
    [SerializeField] private GameObject pauseMenuPanel; // Ana duraklatma menüsü
    [SerializeField] private GameObject saveLoadPanel;  // Save/Load paneli
    [SerializeField] private GameObject settingsPanel;  // Ayarlar paneli

    void Start()
    {
        // Oyunun her zaman tüm paneller kapalı başlamasını garantile.
        Resume();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (IsGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Oyunu devam ettirir (Tüm menüleri kapatır)
    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    // Oyunu duraklatır (Sadece ana duraklatma menüsünü açar)
    void Pause()
    {
        pauseMenuPanel.SetActive(true);
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    // "Save" butonuna basıldığında Save/Load panelini açar
    public void OpenSaveLoadMenu()
    {
        pauseMenuPanel.SetActive(false);
        saveLoadPanel.SetActive(true);
    }

    // "Settings" butonuna basıldığında Ayarlar panelini açar
    public void OpenSettingsMenu()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // "Exit" butonu ana menüye döner
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        SceneManager.LoadScene("AnaMenu");
    }

    // Save/Load veya Settings panelindeki "Geri" butonu için
    public void BackToPauseMenu()
    {
        pauseMenuPanel.SetActive(true);
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
}