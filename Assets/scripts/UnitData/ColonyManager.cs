using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic; // List kullanmak için bu gerekli
using KingdomBug; // Beetle sınıfını tanımak için

public class ColonyManager : MonoBehaviour
{
    public static ColonyManager Instance { get; private set; }

    public int CurrentPopulation { get; private set; }
    public int MaxPopulation { get; private set; }
    
    // --- YENİ EKLENEN KISIM ---
    // Kolonideki tüm böceklerin canlı listesi (Nüfus Defteri)
    public List<Beetle> AllBeetles = new List<Beetle>();
    // --- BİTTİ ---

    public UnityEvent OnPopulationChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    
    // --- YENİ EKLENEN FONKSİYONLAR ---
    /// <summary>
    /// Yeni bir böcek doğduğunda bu fonksiyon onu listeye kaydeder.
    /// </summary>
    public void RegisterBeetle(Beetle beetle)
    {
        if (!AllBeetles.Contains(beetle))
        {
            AllBeetles.Add(beetle);
        }
    }

    /// <summary>
    /// Bir böcek öldüğünde bu fonksiyon onu listeden siler.
    /// </summary>
    public void UnregisterBeetle(Beetle beetle)
    {
        if (AllBeetles.Contains(beetle))
        {
            AllBeetles.Remove(beetle);
        }
    }
    // --- BİTTİ ---
    
    // Geri kalan fonksiyonların (Start, AddPopulation vb.) aynı kalabilir,
    // ama emin olmak için tam halini aşağıya ekliyorum.
    
    void Start()
    {
        CurrentPopulation = 0; 
        MaxPopulation = 5;
        OnPopulationChanged?.Invoke();
    }

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

    public void RemovePopulation(int amount)
    {
        CurrentPopulation -= amount;
        if (CurrentPopulation < 0) CurrentPopulation = 0;
        // Ölüm anında UnregisterBeetle zaten çağrıldığı için burada listeyle oynamıyoruz.
        OnPopulationChanged?.Invoke();
    }

    public void IncreaseMaxPopulation(int amount)
    {
        MaxPopulation += amount;
        OnPopulationChanged?.Invoke();
    }
 
    public void LoadPopulationData(int current, int max)
    {
        this.CurrentPopulation = current;
        this.MaxPopulation = max;
        OnPopulationChanged?.Invoke();
    }
}