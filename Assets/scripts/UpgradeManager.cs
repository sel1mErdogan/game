using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    // Satın alınmış olan tüm geliştirmelerin bir listesini tutar.
    public List<UpgradeData> purchasedUpgrades = new List<UpgradeData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Bir geliştirmenin daha önce satın alınıp alınmadığını kontrol eder.
    public bool IsUpgradePurchased(UpgradeData upgrade)
    {
        return purchasedUpgrades.Contains(upgrade);
    }

    // Yeni bir geliştirme satın alır.
    public void PurchaseUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return;

        // Henüz satın alınmadıysa listeye ekle.
        if (!IsUpgradePurchased(upgrade))
        {
            // 1. Maliyeti envanterden düş (InventoryManager'a bu fonksiyonu ekleyeceğiz)
            // InventoryManager.Instance.SpendResources(upgrade.cost);

            // 2. Geliştirmeyi listeye ekle
            purchasedUpgrades.Add(upgrade);
            Debug.Log(upgrade.upgradeName + " geliştirrmesi satın alındı!");

            // 3. Geliştirmenin etkisini uygula (Bu bir sonraki adımda yapılacak)
            // ApplyUpgradeEffect(upgrade);
        }
    }

    // Bir geliştirmenin satın alınabilmesi için ön koşulların sağlanıp sağlanmadığını kontrol eder.
    public bool CanPurchaseUpgrade(UpgradeData upgrade)
    {
        // 1. Ön koşul geliştirme var mı? Varsa, o satın alınmış mı?
        if (upgrade.requiredUpgrade != null && !IsUpgradePurchased(upgrade.requiredUpgrade))
        {
            return false; // Ön koşul sağlanmadı.
        }

        // 2. Yeterli kaynak var mı?
        // if (!InventoryManager.Instance.HasEnoughResources(upgrade.cost))
        // {
        //     return false; // Yeterli kaynak yok.
        // }

        return true; // Tüm koşullar sağlandı, satın alınabilir.
    }
}