using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using KingdomBug;
using InventorySystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static int CurrentSaveSlot { get; private set; }
    
    private GameData gameData;
    private D dayNightCycle; 
    private const int AUTOSAVE_SLOT_INDEX = 3; 

    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; } 
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
        gameData.saveName = $"Krallık {slotIndex + 1}"; 
        SaveSystem.Save(slotIndex, gameData); 
        gameData.saveName = "Otomatik Kayıt"; 
        SaveSystem.Save(AUTOSAVE_SLOT_INDEX, gameData); 
        SceneManager.LoadScene("OyunSahnesi"); 
    }
    
    public void LoadGame(int slotIndex) 
    { 
        GameData loadedData = SaveSystem.Load(slotIndex); 
        if (loadedData != null) 
        { 
            CurrentSaveSlot = slotIndex; 
            this.gameData = loadedData; 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        } 
    }
    
    public void SaveGame(int slotIndex) 
    { 
        StartCoroutine(SaveRoutine(slotIndex, $"Krallık {slotIndex + 1}")); 
    }
    
    public void Autosave() 
    { 
        StartCoroutine(SaveRoutine(AUTOSAVE_SLOT_INDEX, "Otomatik Kayıt")); 
    }

    private IEnumerator SaveRoutine(int slotIndex, string saveName)
    {
        if (this.gameData == null) 
        {
            Debug.LogError("gameData null olduğu için kaydetme iptal edildi!");
            yield break;
        }

        Debug.LogWarning("--- SAVE BAŞLADI ---");

        Debug.Log("ADIM 1: Oyun verileri toplanıyor (GatherAllData)...");
        GatherAllData();
        Debug.Log("ADIM 1 TAMAMLANDI: Oyun verileri başarıyla toplandı.");

        this.gameData.saveName = saveName;
    
        SaveLoadMenu saveLoadMenu = FindObjectOfType<SaveLoadMenu>(true);
        CanvasGroup panelToHide = null;
        if (saveLoadMenu != null) panelToHide = saveLoadMenu.GetComponent<CanvasGroup>();
        if (panelToHide != null) panelToHide.alpha = 0;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Debug.Log("ADIM 2: Ekran görüntüsü alınıyor...");
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        SaveSystem.SaveScreenshot(slotIndex, screenshot);
        Debug.Log("ADIM 2 TAMAMLANDI: Ekran görüntüsü başarıyla alındı.");

        if (panelToHide != null) panelToHide.alpha = 1;
    
        Debug.Log("ADIM 3: Veriler diske yazılıyor (SaveSystem.Save)...");
        SaveSystem.Save(slotIndex, gameData);
        Debug.Log("ADIM 3 TAMAMLANDI: Veriler başarıyla diske yazıldı.");
        Debug.LogWarning("--- SAVE BAŞARIYLA BİTTİ ---");
    }

    public void SetAllWarriorsToAggressive()
    {
        WarriorBeetleAI[] allWarriors = FindObjectsOfType<WarriorBeetleAI>();
        foreach (var warrior in allWarriors)
        {
            warrior.SetAggressionMode(true);
        }
    }

    public void SetAllWarriorsToDefensive()
    {
        WarriorBeetleAI[] allWarriors = FindObjectsOfType<WarriorBeetleAI>();
        foreach (var warrior in allWarriors)
        {
            warrior.SetAggressionMode(false);
        }
    }
    
    private void GatherAllData() 
    {
        if (dayNightCycle != null) { gameData.currentDay = dayNightCycle.CurrentDay; gameData.timeOfDay = dayNightCycle.timeOfDay; }
        
        if (InventoryManager.Instance != null) 
        { 
            gameData.colonyStockpile.Clear(); 
            var stock = InventoryManager.Instance.GetColonyStockpile(); 
            foreach (var item in stock) 
            { 
                gameData.colonyStockpile.Add(new SerializableItemEntry { itemName = item.Key.itemName, amount = item.Value }); 
            } 
        } 
        
        if (ColonyManager.Instance != null) { gameData.currentPopulation = ColonyManager.Instance.CurrentPopulation; gameData.maxPopulation = ColonyManager.Instance.MaxPopulation; } 
        
        gameData.activeUnits.Clear(); 
        foreach (var beetle in FindObjectsOfType<Beetle>()) 
        { 
            var health = beetle.GetComponent<CanSistemi>(); 
            gameData.activeUnits.Add(new SerializableUnitData { 
                unitName = beetle.gameObject.name.Replace("(Clone)", "").Trim(), 
                position = beetle.transform.position, 
                rotation = beetle.transform.rotation, 
                currentHealth = health != null ? health.MevcutCan : 100 
            }); 
        } 
        
        gameData.builtBuildings.Clear(); 
        foreach (var buildingObj in GameObject.FindGameObjectsWithTag("Building")) 
        { 
            gameData.builtBuildings.Add(new SerializableBuildingData { 
                buildingName = buildingObj.name.Replace("(Clone)","").Trim(), 
                position = buildingObj.transform.position, 
                rotation = buildingObj.transform.rotation 
            }); 
        } 
        
        // --- İNŞAAT ALANLARI İÇİN DÜZELTİLMİŞ KISIM ---
        gameData.constructionSites.Clear(); 
        foreach(var site in FindObjectsOfType<ConstructionSite>()) 
        { 
            gameData.constructionSites.Add(new SerializableConstructionSiteData { 
                buildingName = site.GetBuildingName(), // Sadece ismini alıyoruz
                position = site.transform.position, 
                rotation = site.transform.rotation, 
                currentBuildTime = site.GetCurrentBuildTime() 
            }); 
        } 
        
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); 
        if (playerObj != null) { gameData.playerData = new SerializablePlayerData { position = playerObj.transform.position, rotation = playerObj.transform.rotation }; }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    { 
        if (scene.name == "OyunSahnesi") 
        { 
            dayNightCycle = FindObjectOfType<D>(); 
            if (this.gameData == null) { StartNewGame(0); return; } 
            
            CleanupSceneForLoad(); 
            
            if (dayNightCycle != null) { dayNightCycle.CurrentDay = gameData.currentDay; dayNightCycle.LoadTimeOfDay(gameData.timeOfDay); } 
            
            // --- ENVANTER YÜKLEME KISMI DÜZELTİLDİ ---
            if (InventoryManager.Instance != null && ItemDatabase.Instance != null)
            {
                // Boş bir Dictionary oluştur
                Dictionary<ItemData, int> stockpileToLoad = new Dictionary<ItemData, int>();
                
                // Kayıt dosyasındaki her bir "isim, miktar" çifti için...
                foreach (var savedEntry in gameData.colonyStockpile)
                {
                    // Telefon Rehberi'ni (ItemDatabase) kullanarak isme karşılık gelen asıl ItemData'yı bul
                    ItemData itemData = ItemDatabase.Instance.FindItemByName(savedEntry.itemName);
                    if (itemData != null)
                    {
                        // Eğer bulunduysa, yeni Dictionary'ye ekle
                        stockpileToLoad.Add(itemData, savedEntry.amount);
                    }
                    else
                    {
                        Debug.LogWarning($"Yükleme hatası: '{savedEntry.itemName}' isminde bir eşya ItemDatabase'de bulunamadı!");
                    }
                }
                // Oluşturduğumuz bu tam ve doğru Dictionary'yi envanter yöneticisine yüklet
                // Not: InventoryManager'da LoadStockpile(Dictionary<ItemData, int> ...) fonksiyonu olmalı
                InventoryManager.Instance.LoadStockpile(stockpileToLoad);
            }

            if (ColonyManager.Instance != null) { ColonyManager.Instance.LoadPopulationData(gameData.currentPopulation, gameData.maxPopulation); } 
            
            foreach (var unitData in gameData.activeUnits) 
            { 
                GameObject p = Resources.Load<GameObject>($"Prefabs/Units/{unitData.unitName}"); 
                if (p != null) 
                { 
                    GameObject s = Instantiate(p, unitData.position, unitData.rotation); 
                    s.GetComponent<CanSistemi>()?.LoadHealth(unitData.currentHealth); 
                } 
            } 
            
            foreach (var buildingData in gameData.builtBuildings) 
            { 
                BuildingData d = BuildingDatabase.Instance.FindBuildingByName(buildingData.buildingName); 
                if (d != null && d.finishedBuildingPrefab != null) 
                {
                    Instantiate(d.finishedBuildingPrefab, buildingData.position, buildingData.rotation); 
                }
            } 
            
            // --- İNŞAAT ALANLARI İÇİN DÜZELTİLMİŞ KISIM ---
            foreach(var siteData in gameData.constructionSites) 
            { 
                BuildingData d = BuildingDatabase.Instance.FindBuildingByName(siteData.buildingName); 
                if(d != null && d.constructionSitePrefab != null) 
                { 
                    GameObject s = Instantiate(d.constructionSitePrefab, siteData.position, siteData.rotation); 
                    // İnşaat alanına ismini, ilerlemesini ve maksimum süresini veriyoruz
                    s.GetComponent<ConstructionSite>()?.LoadProgress(d.buildingName, siteData.currentBuildTime, d.buildTime); 
                } 
            } 
            
            StartCoroutine(SetPlayerPosition_Coroutine()); 
            ProcessCollectedItems(); 
        } 
    }
    
    private IEnumerator SetPlayerPosition_Coroutine() 
    { 
        yield return new WaitForEndOfFrame(); 
        if (gameData.playerData != null) 
        { 
            GameObject player = GameObject.FindGameObjectWithTag("Player"); 
            if (player != null) 
            { 
                var moveScript = player.GetComponent<playerMovements>(); 
                var rb = player.GetComponent<Rigidbody>(); 
                if (moveScript != null) moveScript.enabled = false; 
                if (rb != null) 
                { 
                    rb.velocity = Vector3.zero; 
                    rb.angularVelocity = Vector3.zero; 
                    rb.position = gameData.playerData.position; 
                    rb.rotation = gameData.playerData.rotation; 
                } 
                else 
                { 
                    player.transform.position = gameData.playerData.position; 
                    player.transform.rotation = gameData.playerData.rotation; 
                } 
                yield return new WaitForFixedUpdate(); 
                if (moveScript != null) moveScript.enabled = true; 
            } 
        } 
    }
    
    private void CleanupSceneForLoad() 
    { 
        foreach (var b in FindObjectsOfType<Beetle>()) Destroy(b.gameObject); 
        foreach(var b in GameObject.FindGameObjectsWithTag("Building")) Destroy(b); 
        foreach(var s in FindObjectsOfType<ConstructionSite>()) Destroy(s.gameObject); 
    }
    
    private void Update() { }
    
    public void NotifyItemCollected(string id) 
    { 
        if (gameData != null && !gameData.collectedWorldItemIDs.Contains(id)) 
        {
            gameData.collectedWorldItemIDs.Add(id); 
        }
    }
    
    private void ProcessCollectedItems() 
    { 
        WorldItem[] items = FindObjectsOfType<WorldItem>(); 
        foreach(var i in items) 
        {
            if(gameData.collectedWorldItemIDs.Contains(i.GetID())) 
            {
                Destroy(i.gameObject); 
            }
        }
    }
}