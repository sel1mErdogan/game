using UnityEngine;
using System.Collections.Generic;

// Unity'de sağ tıklayıp Create > KingdomBug > Unit Data menüsünden
// yeni böcek üretim verileri oluşturmak için kullanılır.
[CreateAssetMenu(fileName = "New Unit Data", menuName = "KingdomBug/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Birim Bilgileri")]
    public string unitName; // Örn: "İşçi Böcek"
    public GameObject unitPrefab; // Üretilecek böceğin prefab'ı
    public Sprite unitIcon; // Üretim menüsünde görünecek ikon

    [Header("Üretim Gereksinimleri")]
    // GDD'ye göre buraya sadece "Kırıntı" ekleyeceğiz.
    public List<ResourceCost> productionCost; 
    public float productionTime = 5f; // Üretimin kaç saniye süreceği
    public int populationCost = 1; // Nüfus limitinde kapladığı yer
}