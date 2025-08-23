using UnityEngine;
using TMPro; // TextMeshPro kullanacağımız için bu satır önemli

public class PopulationUI : MonoBehaviour
{
    [Tooltip("Nüfus bilgisini gösterecek olan TextMeshPro objesi")]
    [SerializeField] private TextMeshProUGUI populationText;

    // Bu script aktif olduğunda, ColonyManager'daki event'e abone olur.
    private void OnEnable()
    {
        if (ColonyManager.Instance != null)
        {
            ColonyManager.Instance.OnPopulationChanged.AddListener(UpdatePopulationText);
            // Başlangıçta da bir kere güncelleyerek doğru değeri göstermesini sağla
            UpdatePopulationText();
        }
    }

    // Script kapandığında veya obje yok olduğunda abonelikten çıkar.
    private void OnDisable()
    {
        if (ColonyManager.Instance != null)
        {
            ColonyManager.Instance.OnPopulationChanged.RemoveListener(UpdatePopulationText);
        }
    }

    // PopulationUI.cs

    public void UpdatePopulationText()
    {
        if (populationText != null && ColonyManager.Instance != null)
        {
            int currentPop = ColonyManager.Instance.CurrentPopulation;
            int maxPop = ColonyManager.Instance.MaxPopulation;
            populationText.text = $"Nüfus: {currentPop} / {maxPop}";
        
            // YENİ EKLENDİ: Bu fonksiyonun ne zaman ve hangi değerlerle çalıştığını görelim.
            Debug.Log($"Population UI güncellendi: {currentPop} / {maxPop}");
        }
    }
}