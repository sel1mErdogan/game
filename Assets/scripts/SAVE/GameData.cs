using System.Collections.Generic;
using UnityEngine;

// --- KAYIT SİSTEMİ İÇİN GEREKLİ YARDIMCI VERİ SINIFLARI ---
// Bu sınıfların hepsi [System.Serializable] olmalı ve karmaşık referanslar içermemeli.

[System.Serializable]
public class SerializableItemEntry
{
    public string itemName;
    public int amount;
}

[System.Serializable]
public class SerializableUnitData
{
    public string unitName;
    public Vector3 position;
    public Quaternion rotation;
    public int currentHealth;
}

[System.Serializable]
public class SerializableBuildingData
{
    public string buildingName;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable] 
public class SerializableConstructionSiteData 
{ 
    public string buildingName; // Hangi binanın inşa edildiğinin adı
    public Vector3 position; 
    public Quaternion rotation; 
    public float currentBuildTime; // İnşaatın ne kadar ilerlediği
}

[System.Serializable]
public class SerializablePlayerData
{
    public Vector3 position;
    public Quaternion rotation;
}


// --- ANA OYUN VERİ SINIFI ---
[System.Serializable]
public class GameData
{
    public string saveName;
    public string lastSaved;
    public int currentDay;
    public float timeOfDay;
    public List<SerializableItemEntry> colonyStockpile;
    public int currentPopulation;
    public int maxPopulation;
    public List<string> collectedWorldItemIDs;
    public List<SerializableUnitData> activeUnits;
    public List<SerializableBuildingData> builtBuildings;
    public List<SerializableConstructionSiteData> constructionSites;
    public SerializablePlayerData playerData;

    public GameData()
    {
        this.saveName = "Boş Yuva";
        this.lastSaved = "";
        this.currentDay = 1;
        this.timeOfDay = 0.25f;
        this.colonyStockpile = new List<SerializableItemEntry>();
        this.currentPopulation = 0; // Başlangıç nüfusu 0 olmalı, sahnedekiler sayılır.
        this.maxPopulation = 10;
        this.collectedWorldItemIDs = new List<string>();
        this.activeUnits = new List<SerializableUnitData>();
        this.builtBuildings = new List<SerializableBuildingData>();
        this.constructionSites = new List<SerializableConstructionSiteData>();
        this.playerData = null; 
    }
}