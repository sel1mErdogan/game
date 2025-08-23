using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private static readonly string SAVE_FILE_FORMAT = "gameData_{0}.json";
    public const int MAX_SAVE_SLOTS = 3;

    private static string GetSavePath(int slotIndex)
    {
        return SAVE_FOLDER + string.Format(SAVE_FILE_FORMAT, slotIndex);
    }

    public static void Save(int slotIndex, GameData data)
    {
        if (!Directory.Exists(SAVE_FOLDER)) Directory.CreateDirectory(SAVE_FOLDER);
        
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
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        return null;
    }

    public static void Delete(int slotIndex)
    {
        string filePath = GetSavePath(slotIndex);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Kayıt dosyası {slotIndex} silindi.");
        }
    }

    public static List<GameData> LoadAllSaveProfiles()
    {
        if (!Directory.Exists(SAVE_FOLDER)) Directory.CreateDirectory(SAVE_FOLDER);

        List<GameData> allProfiles = new List<GameData>();
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            allProfiles.Add(Load(i));
        }
        return allProfiles;
    }
}
