// CanSistemi.cs (Güncellenmiş hali)

using UnityEngine;

public class CanSistemi : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxCan = 100;
    private int mevcutCan;

    // YENİ EKLENDİ: Can barı UI'ına referans
    [Header("UI Referansı")]
    [SerializeField] private DusmanCanBariUI canBariUI;

    void Start()
    {
        mevcutCan = maxCan;
        
        // Başlangıçta can barını tam can olarak ayarla
        if (canBariUI != null)
        {
            canBariUI.CaniGuncelle(mevcutCan, maxCan);
        }
    }

    public void HasarAl(int hasarMiktari)
    {
        mevcutCan -= hasarMiktari;
        if (mevcutCan < 0) mevcutCan = 0; // Canın eksiye düşmesini engelle

        Debug.Log(gameObject.name + " " + hasarMiktari + " hasar aldı. Kalan can: " + mevcutCan);

        // YENİ EKLENDİ: Can barını güncelle
        if (canBariUI != null)
        {
            canBariUI.CaniGuncelle(mevcutCan, maxCan);
        }

        if (mevcutCan <= 0)
        {
            Ol();
        }
    }

    private void Ol()
    {
        Debug.Log(gameObject.name + " öldü!");
        Destroy(gameObject);
    }
}