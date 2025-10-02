using UnityEngine;

// Script'in adını ItemVisuals olarak değiştirdik, çünkü artık sadece döndürmüyor.
public class ItemVisuals : MonoBehaviour
{
    // --- DÖNME AYARLARI ---
    [Header("Dönme Ayarları")]
    public float donusHizi = 30f;

    // --- GÖRSEL KÜRE AYARLARI ---
    [Header("Görsel Küre Ayarları")]
    public GameObject spherePrefab; // Inspector'dan sürükleyeceğimiz küre prefab'ı
    public float sphereScale = 1.5f; // Kürenin ne kadar büyük olacağı

    void Start()
    {
        // --- KÜRE OLUŞTURMA KODU ---
        // Oyun başladığında, eğer bir küre prefab'ı atanmışsa onu oluşturur.
        if (spherePrefab != null)
        {
            GameObject sphereInstance = Instantiate(spherePrefab, transform.position, transform.rotation);
            sphereInstance.transform.SetParent(this.transform);
            sphereInstance.transform.localPosition = Vector3.zero; // Kürenin merkezini tam olarak item'in merkezine ayarlar
            sphereInstance.transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale);
        }
    }

    void Update()
    {
        // --- DÖNDÜRME KODU ---
        // Her frame'de objeyi kendi Y ekseni etrafında döndürür.
        transform.Rotate(Vector3.up, donusHizi * Time.deltaTime);
    }
}