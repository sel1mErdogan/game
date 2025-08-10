using UnityEngine;
using System.Collections.Generic;

// Bu script'i kullanarak Unity'de sağ tıklayıp Create > KingdomBug > Building Data
// menüsünden yeni yapı verileri (Kürdan Kulesi, Kalkan vs.) oluşturacağız.
[CreateAssetMenu(fileName = "New Building Data", menuName = "KingdomBug/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("Genel Bilgiler")]
    public string buildingName;
    [TextArea] public string description;
    public Sprite buildingIcon;
    
    [Header("İnşaat Bilgileri")]
    public List<ResourceCost> requiredMaterials;
    public float buildTime = 10f;

    [Header("Obje Referansları")]
    public GameObject placementGhostPrefab; // Yerleştirirken görünecek hayalet model
    public GameObject constructionSitePrefab; // İnşaat alanı modeli (iskele vb.)
    public GameObject finishedBuildingPrefab; // Tamamlanmış yapı modeli

    [Header("Özel Fonksiyonlar (GDD'ye göre)")]
    public bool hasDurability = false; // Kalkan gibi dayanıklılığı var mı?
    public bool needsAmmo = false; // Kibrit Fırlatıcı gibi mühimmat gerekiyor mu?
}