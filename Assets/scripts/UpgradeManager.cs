using UnityEngine;
using System.Collections.Generic;
using InventorySystem;
using KingdomBug;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CanPurchaseUpgrade(UpgradeData upgrade, Beetle targetBeetle)
    {
        if (upgrade.requiredUpgrade != null && !targetBeetle.HasUpgrade(upgrade.requiredUpgrade))
        {
            return false; // Gerekli olan önceki geliştirmeye sahip değil.
        }
        
        // TODO: Kaynak kontrolü de buraya eklenebilir. Şimdilik true dönüyoruz.
        return true; 
    }

    public bool PurchaseUpgrade(UpgradeData upgrade, Beetle targetBeetle)
    {
        if (upgrade == null || targetBeetle == null || targetBeetle.HasUpgrade(upgrade) || !CanPurchaseUpgrade(upgrade, targetBeetle))
        {
            return false;
        }

        if (InventoryManager.Instance.SpendResources(new List<ResourceCost>(upgrade.cost)))
        {
            targetBeetle.AddUpgrade(upgrade);
            ApplyUpgradeEffect(upgrade, targetBeetle);
            Debug.Log(targetBeetle.name + " için '" + upgrade.upgradeName + "' geliştirrmesi satın alındı!");
            return true;
        }
        
        Debug.LogWarning(upgrade.upgradeName + " için kaynaklar yetersiz!");
        return false;
    }
    
    private void ApplyUpgradeEffect(UpgradeData upgrade, Beetle targetBeetle)
    {
        switch (upgrade.effectType)
        {
            case UpgradeEffectType.IncreaseMaxHealth:
                targetBeetle.GetComponent<CanSistemi>()?.IncreaseMaxHealth((int)upgrade.effectValue);
                break;
            case UpgradeEffectType.IncreaseInventorySize:
                targetBeetle.IncreaseInventorySize((int)upgrade.effectValue);
                break;
            case UpgradeEffectType.IncreaseMoveSpeed:
                targetBeetle.GetComponent<UnityEngine.AI.NavMeshAgent>().speed += upgrade.effectValue;
                break;
            case UpgradeEffectType.IncreaseAttackDamage:
                targetBeetle.GetComponent<WarriorBeetleAI>()?.IncreaseDamage((int)upgrade.effectValue);
                break;
            case UpgradeEffectType.DecreaseAttackCooldown:
                targetBeetle.GetComponent<WarriorBeetleAI>()?.DecreaseAttackCooldown(upgrade.effectValue);
                break;
        }
    }
}