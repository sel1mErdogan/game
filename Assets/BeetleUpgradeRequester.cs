using UnityEngine;
using System.Collections.Generic;
using KingdomBug;

public class BeetleUpgradeRequester : MonoBehaviour
{
    [Header("Bağlantılar")]
    [SerializeField] private BeetleExperience experience;
    [SerializeField] private Beetle beetle;

    [Header("Geliştirme Ayarları")]
    public List<UpgradeData> possibleUpgrades;
    
    private UpgradeData requestedUpgrade;
    public bool HasRequest => requestedUpgrade != null; // Dışarıdan talebi var mı diye kontrol etmek için

    void Awake()
    {
        if (experience == null) experience = GetComponent<BeetleExperience>();
        if (beetle == null) beetle = GetComponent<Beetle>();
    }

    void OnEnable() { experience.OnLevelUp += HandleLevelUp; }
    void OnDisable() { experience.OnLevelUp -= HandleLevelUp; }

    private void HandleLevelUp()
    {
        // KURAL: Sadece 5. seviye ve üzeri için talep oluştur
        if (experience.level >= 5 && !HasRequest) // Zaten bir talebi yoksa
        {
            foreach (var upgrade in possibleUpgrades)
            {
                if (!beetle.HasUpgrade(upgrade))
                {
                    requestedUpgrade = upgrade;
                    Debug.Log(beetle.name + ", 5. seviyeye ulaştı ve yeni bir talep oluşturdu: " + requestedUpgrade.upgradeName);
                    // DİKKAT: Artık UI'a haber vermiyoruz. Sadece talebi aklımızda tutuyoruz.
                    // UI paneli açıldığında bizi kontrol edecek.
                    return; 
                }
            }
        }
    }
    
    public UpgradeData GetRequestedUpgrade() => requestedUpgrade;

    // Talep onaylandığında veya reddedildiğinde bu çağrılacak
    public void ClearRequest() 
    {
        requestedUpgrade = null;
    }
}