using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private static readonly string SAVE_FILE_FORMAT = "gameData_{0}.json";
    public const int MAX_SAVE_SLOTS = 4; // 3 Manuel + 1 Otomatik Kayıt

    private static void Init()
    {
        if (!Directory.Exists(SAVE_FOLDER)) Directory.CreateDirectory(SAVE_FOLDER);
    }

    private static string GetSavePath(int slotIndex)
    {
        return SAVE_FOLDER + string.Format(SAVE_FILE_FORMAT, slotIndex);
    }
    
    // --- JSON VERİ İŞLEMLERİ ---

    public static void Save(int slotIndex, GameData data)
    {
        Init();
        data.lastSaved = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slotIndex), json);
        Debug.Log($"Oyun {slotIndex} numaralı yuvaya kaydedildi.");
    }

    public static GameData Load(int slotIndex)
    {
        string filePath = GetSavePath(slotIndex);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    public static void Delete(int slotIndex)
    {
        // JSON verisini sil
        string filePath = GetSavePath(slotIndex);
        if (File.Exists(filePath)) File.Delete(filePath);
        
        // Ekran görüntüsünü de sil
        string screenshotPath = SAVE_FOLDER + $"screenshot_{slotIndex}.png";
        if (File.Exists(screenshotPath)) File.Delete(screenshotPath);
        
        Debug.Log($"Kayıt dosyası {slotIndex} ve ilgili tüm veriler silindi.");
    }

    public static List<GameData> LoadAllSaveProfiles()
    {
        Init();
        List<GameData> allProfiles = new List<GameData>();
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            allProfiles.Add(Load(i)); 
        }
        return allProfiles;
    }

    // --- EKRAN GÖRÜNTÜSÜ İŞLEMLERİ (YENİ) ---

    public static void SaveScreenshot(int slotIndex, Texture2D screenshot)
    {
        Init();
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(SAVE_FOLDER + $"screenshot_{slotIndex}.png", bytes);
        Debug.Log($"Ekran görüntüsü {slotIndex} numaralı yuva için kaydedildi.");
    }

    public static Texture2D LoadScreenshot(int slotIndex)
    {
        string path = SAVE_FOLDER + $"screenshot_{slotIndex}.png";
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D screenshot = new Texture2D(2, 2);
            screenshot.LoadImage(bytes);
            return screenshot;
        }
        return null;
    }
}