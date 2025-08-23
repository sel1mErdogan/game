// MainMenuUI.cs (Sadeleştirilmiş Hali)

using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        BackToMainMenu();
    }

    // "Play" butonu artık SADECE oyun sahnesini yükleyecek.
    // Geri kalan her şeyi GameManager halledecek.
    public void OnPlayButton()
    {
        // GameManager'daki karmaşık fonksiyonları sildiğimiz için
        // eski kod hata veriyordu. Şimdi sadece oyun sahnesini yüklüyoruz.
        SceneManager.LoadScene("OyunSahnesi");
    }

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