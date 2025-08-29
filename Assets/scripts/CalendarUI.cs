using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CalendarUI : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private D dayNightSystem;
    [SerializeField] private TextMeshProUGUI dayCounterText;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject dayMarkerPrefab;

    [Header("Ayarlar")]
    [SerializeField] private int totalDaysToShow = 15;
    [SerializeField] private Color pastDayColor = Color.green;
    [SerializeField] private Color currentDayColor = Color.yellow;
    [SerializeField] private Color futureDayColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private List<Image> dayMarkers = new List<Image>();

    void Start()
    {
        // 1. Gün/Gece sistemini bulmaya çalış
        if (dayNightSystem == null)
        {
            Debug.Log("[Takvim] DayNightSystem referansı boş, sahnede aranıyor...");
            dayNightSystem = FindObjectOfType<D>();
        }

        if (dayNightSystem == null)
        {
            Debug.LogError("[HATA] Sahnede D.cs script'ine sahip bir obje bulunamadı! Takvim çalışamaz.");
            return; // Sistem bulunamadıysa devam etme
        }
        else
        {
            Debug.Log("[Takvim] DayNightSystem başarıyla bulundu!");
        }
        
        // 2. Takvim kutucuklarını oluştur
        CreateCalendarMarkers();
        
        // 3. Gün döngüsündeki "gün başladı" event'ine bizim fonksiyonumuzu bağla
        dayNightSystem.OnDayStart.AddListener(UpdateCalendar);
        Debug.Log("[Takvim] 'OnDayStart' event'ine başarıyla abone olundu.");
        
        // 4. Oyuna başlarken de takvimi bir kere güncelle
        UpdateCalendar();
        
        // Başlangıçta panelin kapalı olduğundan emin ol
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(dayNightSystem != null)
        {
            dayNightSystem.OnDayStart.RemoveListener(UpdateCalendar);
        }
    }
    
    public void TogglePanel()
    {
        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);

        if (!isActive)
        {
            Debug.Log("[Takvim] Panel açıldı, takvim güncelleniyor...");
            UpdateCalendar();
        }
    }

    public void UpdateCalendar()
    {
        if (dayNightSystem == null)
        {
            Debug.LogError("[HATA] UpdateCalendar çağrıldı ama DayNightSystem referansı BOŞ!");
            return;
        }

        int currentDay = dayNightSystem.CurrentDay;
        Debug.Log($"[Takvim] Güncelleme başladı. Mevcut Gün: {currentDay}");
        
        if (dayCounterText != null) dayCounterText.text = currentDay.ToString();

        for (int i = 0; i < dayMarkers.Count; i++)
        {
            // Bu döngünün çalıştığını doğrulamak için sadece bir kere log yazdıralım
            if(i == 0) Debug.Log($"[Takvim] Renkler ayarlanıyor... Toplam {dayMarkers.Count} kutucuk var.");

            if (i < currentDay - 1) dayMarkers[i].color = pastDayColor;
            else if (i == currentDay - 1) dayMarkers[i].color = currentDayColor;
            else dayMarkers[i].color = futureDayColor;
        }
    }

    private void CreateCalendarMarkers()
    {
        if (dayMarkerPrefab == null)
        {
            Debug.LogError("[HATA] dayMarkerPrefab referansı BOŞ! Lütfen Inspector'dan atama yapın.");
            return;
        }

        for (int i = 0; i < totalDaysToShow; i++)
        {
            GameObject markerObj = Instantiate(dayMarkerPrefab, gridParent);
            dayMarkers.Add(markerObj.GetComponent<Image>());
        }
        Debug.Log($"[Takvim] {totalDaysToShow} adet gün kutucuğu başarıyla oluşturuldu.");
    }
}