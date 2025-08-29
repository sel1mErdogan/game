using UnityEngine;

// Bu enum, bir geliştirmenin böceğin HANGİ özelliğini etkileyeceğini belirler.
public enum UpgradeEffectType
{
    IncreaseMaxHealth,      // Maksimum canı artır
    IncreaseInventorySize,  // Envanter boyutunu artır
    IncreaseMoveSpeed,      // Hareket hızını artır
    IncreaseAttackDamage,   // Saldırı hasarını artır
    DecreaseAttackCooldown, // Saldırı hızını artır (bekleme süresini düşür)
    // Gelecekte buraya yeni etki türleri ekleyebiliriz (örn: AddPoisonEffect)
}

[CreateAssetMenu(fileName = "Yeni Geliştirme", menuName = "Geliştirme Sistemi/Yeni Geliştirme")]
public class UpgradeData : ScriptableObject
{
    [Header("Geliştirme Bilgileri")]
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Gereksinimler")]
    public ResourceCost[] cost; // Bu geliştirmeyi satın almanın maliyeti
    public UpgradeData requiredUpgrade; // Bu geliştirmeyi açmak için önce hangi geliştirmenin alınması gerektiği

    [Header("Geliştirmenin Etkisi")]
    public UpgradeEffectType effectType; // Bu geliştirme ne işe yarar?
    public float effectValue; // Etkinin değeri (örn: +10 can, +2 envanter)
}