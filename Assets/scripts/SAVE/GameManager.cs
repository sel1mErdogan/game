using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int CurrentSaveSlot { get; private set; }
    private GameData gameData;
    private D dayNightCycle;

    // --- YENİ EKLENDİ ---
    private const string LastUsedSlotKey = "LastUsedSlot"; // Not defterinin adı

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartNewGame(int slotIndex)
    {
        CurrentSaveSlot = slotIndex;
        gameData = new GameData();
        gameData.saveName = "Yeni Krallık " + (slotIndex + 1);
        SaveSystem.SaveGame(CurrentSaveSlot, gameData);

        // --- YENİ EKLENDİ: Not al ---
        PlayerPrefs.SetInt(LastUsedSlotKey, slotIndex);

        SceneManager.LoadScene("OyunSahnesi");
    }

    public void ContinueGame(int slotIndex)
    {
        CurrentSaveSlot = slotIndex;
        
        // --- YENİ EKLENDİ: Not al ---
        PlayerPrefs.SetInt(LastUsedSlotKey, slotIndex);

        SceneManager.LoadScene("OyunSahnesi");
    }

    public void SaveCurrentGame()
    {
        if (gameData == null) return;
        if (dayNightCycle != null)
        {
            gameData.currentDay = dayNightCycle.CurrentDay;
        }
        SaveSystem.SaveGame(CurrentSaveSlot, gameData);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "OyunSahnesi")
        {
            dayNightCycle = FindObjectOfType<D>();
            gameData = SaveSystem.LoadGame(CurrentSaveSlot);

            if (gameData != null && dayNightCycle != null)
            {
                dayNightCycle.CurrentDay = gameData.currentDay;
            }
            else if (gameData == null)
            {
                gameData = new GameData();
                if (dayNightCycle != null)
                {
                    dayNightCycle.CurrentDay = gameData.currentDay;
                }
            }
        }
    }
    // GameManager.cs'in içine bu yeni fonksiyonu ekle.
// Diğer fonksiyonlar aynı kalacak.

    public void SaveCurrentGame(int slotIndex)
    {
        // Hangi yuvaya kaydedeceğimizi güncelle
        CurrentSaveSlot = slotIndex;
    
        // Mevcut oyunu kaydet
        SaveCurrentGame(); // Bu, zaten yazdığımız eski fonksiyonu çağırıyor.
    }
    public void NotifyItemCollected(string id)
    {
        // Bu fonksiyon şimdilik sadece WorldItem'daki hatayı gidermek için var.
        // Henüz toplanan item'ları kaydetme mantığı burada ekli değil.
        
    }

}