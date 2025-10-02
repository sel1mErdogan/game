using UnityEngine;
using System; // Event kullanabilmek için bu gerekli.

public class BeetleExperience : MonoBehaviour
{
    [Header("Mevcut Durum")]
    public int level = 1;
    public int currentXP = 0;

    [Header("Ayarlar")]
    public int xpToNextLevel = 100;
    public float xpMultiplier = 1.5f;

    // Seviye atlandığında diğer script'lere haber verecek olan sinyalimiz.
    public event Action OnLevelUp;

    /// <summary>
    /// Bu böceğe tecrübe puanı ekler.
    /// </summary>
    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log(gameObject.name + ", " + amount + " XP kazandı. (Toplam: " + currentXP + "/" + xpToNextLevel + ")");
        
        // Seviye atlama kontrolü
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel; // Fazla XP'yi sonraki seviyeye aktar
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
        
        Debug.LogWarning(gameObject.name + " SEVİYE ATLADI! Yeni seviye: " + level);
        
        // Sinyali gönder! "Ben seviye atladım!"
        OnLevelUp?.Invoke();
    }
}