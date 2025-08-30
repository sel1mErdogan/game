using UnityEngine;
using System.Collections.Generic;
using InventorySystem; // InventoryManager'a erişim için bunu ekle

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    public List<UpgradeData> purchasedUpgrades = new List<UpgradeData>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsUpgradePurchased(UpgradeData upgrade)
    {
        if (upgrade == null) return false;
        return purchasedUpgrades.Contains(upgrade);
    }

    // Satın alma işlemini dener ve başarılı olup olmadığını döndürür (true/false)
    public bool PurchaseUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null || !CanPurchaseUpgrade(upgrade) || IsUpgradePurchased(upgrade)) 
        {
            return false; // Satın alınamazsa veya zaten alınmışsa işlemi iptal et
        }

        // 1. Maliyeti envanterden düşmeyi dene
        if (InventoryManager.Instance.SpendResources(new List<ResourceCost>(upgrade.cost)))
        {
            // 2. Geliştirmeyi "satın alındı" olarak listeye ekle
            purchasedUpgrades.Add(upgrade);
            Debug.Log(upgrade.upgradeName + " geliştirrmesi satın alındı!");
            
            // 3. Geliştirmenin etkisini uygula (Burası bir sonraki adımda yapılacak en önemli kısım!)
            // ApplyUpgradeEffect(upgrade);
            
            return true; // İşlem başarılı
        }
        
        Debug.LogWarning("Yeterli kaynak olmadığı için satın alım başarısız!");
        return false; // Kaynak harcanamadıysa başarısız
    }

    public bool CanPurchaseUpgrade(UpgradeData upgrade)
    {
        if (upgrade.requiredUpgrade != null && !IsUpgradePurchased(upgrade.requiredUpgrade))
        {
            return false;
        }
        
        // Bu fonksiyon, butonun en baştan inaktif görünmesi için bir ön kontrol yapar.
        // Gerçek harcama işlemi PurchaseUpgrade içinde yapılır.
        // InventoryManager'a bunun için yeni bir fonksiyon ekleyebiliriz veya
        // mevcut mantığa güvenebiliriz. Şimdilik bu haliyle bırakalım.

        return true;
    }
}