// DusmanCanBariUI.cs

using UnityEngine;
using UnityEngine.UI; // Slider'ı kullanmak için bu kütüphane gerekli

public class DusmanCanBariUI : MonoBehaviour
{
    [Tooltip("Kontrol edilecek Slider UI elemanı")]
    public Slider canBariSlider;
    
    [Tooltip("UI'ın bakacağı kamera")]
    public Transform hedefKamera;

    void Start()
    {
        // Eğer kamera atanmamışsa, ana kamerayı bul
        if (hedefKamera == null)
        {
            hedefKamera = Camera.main.transform;
        }
    }

    // LateUpdate, tüm hareket ve kamera işlemleri bittikten sonra çalışır.
    // Bu sayede UI'ın titremesi engellenir.
    void LateUpdate()
    {
        // Canvas'ın her zaman kameraya bakmasını sağla
        if (hedefKamera != null)
        {
            transform.LookAt(transform.position + hedefKamera.rotation * Vector3.forward,
                             hedefKamera.rotation * Vector3.up);
        }
    }

    // Can değerini güncelleyen public fonksiyon
    // Bu fonksiyonu CanSistemi script'inden çağıracağız
    public void CaniGuncelle(int mevcutCan, int maxCan)
    {
        // Slider'ın değerini mevcut can / max can oranına göre ayarla (0 ile 1 arasında bir değer)
        canBariSlider.value = (float)mevcutCan / (float)maxCan;
    }
}