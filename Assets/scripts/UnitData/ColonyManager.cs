using UnityEngine;
using UnityEngine.Events; // UI güncellemesi için

public class ColonyManager : MonoBehaviour
{
    public static ColonyManager Instance { get; private set; }

    public int CurrentPopulation { get; private set; }
    public int MaxPopulation { get; private set; }

    // UI'ın nüfus değişimini dinlemesi için bir event
    public UnityEvent OnPopulationChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        // Başlangıç değerleri
        CurrentPopulation = 0; // Sahnedeki başlangıç böceklerini saydırabiliriz
        MaxPopulation = 5; // Oyuna başlarkenki limit
        OnPopulationChanged?.Invoke();
    }

    // Yeni bir böcek üretildiğinde çağrılır
    public bool AddPopulation(int amount)
    {
        if (CurrentPopulation + amount <= MaxPopulation)
        {
            CurrentPopulation += amount;
            OnPopulationChanged?.Invoke();
            return true;
        }
        else
        {
            Debug.LogWarning("Nüfus limiti dolu!");
            return false;
        }
    }

    // Bir böcek öldüğünde çağrılır
    public void RemovePopulation(int amount)
    {
        CurrentPopulation -= amount;
        if (CurrentPopulation < 0) CurrentPopulation = 0;
        OnPopulationChanged?.Invoke();
    }

    // Yeni bir "Yuva" inşa edildiğinde çağrılır
    public void IncreaseMaxPopulation(int amount)
    {
        MaxPopulation += amount;
        OnPopulationChanged?.Invoke();
    }
}