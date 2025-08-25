using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderController : MonoBehaviour
{
    public enum VolumeType { Music, SFX }
    [Tooltip("Bu slider'ın Müzik sesini mi yoksa Efekt sesini mi kontrol edeceğini seçin.")]
    public VolumeType volumeType;

    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        
        // 1. Slider'ın başlangıç değerini kayıtlı veriden oku
        LoadSliderValue();
        
        // 2. Slider'ın değeri her değiştiğinde hangi fonksiyonu çağıracağını belirle
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void LoadSliderValue()
    {
        string prefKey = (volumeType == VolumeType.Music) ? "MusicVolumePreference" : "SFXVolumePreference";
        slider.value = PlayerPrefs.GetFloat(prefKey, 1f);
    }

    private void OnSliderValueChanged(float value)
    {
        // Değer değiştiğinde, doğru fonksiyonu çağırarak "emir merkezine" haber ver
        if (volumeType == VolumeType.Music)
        {
            SettingsManager.Instance.SetMusicVolume(value);
        }
        else
        {
            SettingsManager.Instance.SetSFXVolume(value);
        }
    }
}