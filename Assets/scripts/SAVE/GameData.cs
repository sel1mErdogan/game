// --- GameData.cs (Yazım Hatası Düzeltilmiş Nihai Versiyon) ---
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string saveName;
    public string lastSaved;

    // Genel Durum
    public int currentDay;
    public float timeOfDay;
    // Envanter
    public List<SerializableItemEntry> colonyStockpile;
    // Nüfus
    public int currentPopulation;
    public int maxPopulation;
    // Dünya Durumu
    public List<string> collectedWorldItemIDs;
    // Sahnedeki Birimler (Böcekler)
    public List<SerializableUnitData> activeUnits;
    // İnşa Edilmiş Binalar
    public List<SerializableBuildingData> builtBuildings;
    // İnşaat Halindeki Alanlar
    public List<SerializableConstructionSiteData> constructionSites;
    // Oyuncu Verisi
    public SerializablePlayerData playerData;

    public GameData()
    {
        this.saveName = "Boş Yuva";
        this.lastSaved = "";
        this.currentDay = 1;
        this.timeOfDay = 0.25f;
        this.colonyStockpile = new List<SerializableItemEntry>();
        this.currentPopulation = 5;
        this.maxPopulation = 10;
        this.collectedWorldItemIDs = new List<string>();
        this.activeUnits = new List<SerializableUnitData>();
        this.builtBuildings = new List<SerializableBuildingData>();
        this.constructionSites = new List<SerializableConstructionSiteData>();
        this.playerData = null; 
    }
}

// --- Kayıt Sistemi İçin Gerekli Yardımcı Veri Sınıfları ---

[System.Serializable] public class SerializableItemEntry { public string itemName; public int amount; }
[System.Serializable] public class SerializableUnitData { public string unitName; public Vector3 position; public Quaternion rotation; public int currentHealth; }
[System.Serializable] public class SerializableBuildingData { public string buildingName; public Vector3 position; public Quaternion rotation; }

// DÜZELTİLMİŞ SINIF BURADA
[System.Serializable] 
public class SerializableConstructionSiteData 
{ 
    public string buildingName; // Sadece bir tane olacak
    public Vector3 position; 
    public Quaternion rotation; 
    public float currentBuildTime; 
}

[System.Serializable] public class SerializablePlayerData { public Vector3 position; public Quaternion rotation; }