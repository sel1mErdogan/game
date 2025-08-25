using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private AudioMixer mainMixer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Oyun ilk açıldığında kayıtlı ayarları miksere uygula
        LoadInitialSettings();
    }

    // Sadece mikserin değerini değiştirir ve değişikliği kaydeder
    public void SetMusicVolume(float volume)
    {
        if (volume < 0.0001f) volume = 0.0001f;
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolumePreference", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (volume < 0.0001f) volume = 0.0001f;
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolumePreference", volume);
    }
    
    // Oyun ilk açıldığında ayarları yükler
    private void LoadInitialSettings()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolumePreference", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolumePreference", 1f);
        
        // Sadece mikseri ayarla, UI'a karışma
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);
    }

    // Grafik ayarları aynı kalabilir
    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("GraphicsQualityPreference", qualityIndex);
    }
}