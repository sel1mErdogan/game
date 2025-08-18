using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class SaveSystem
{
    private static readonly string saveFileNameFormat = "/gameData_{0}.json";
    public static readonly int maxSaveSlots = 3;

    public static void SaveGame(int slotIndex, GameData data)
    {
        if (string.IsNullOrEmpty(data.saveName))
        {
            data.saveName = $"My Kingdom {slotIndex + 1}";
        }
        data.lastSaved = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + string.Format(saveFileNameFormat, slotIndex);
        File.WriteAllText(path, json);
    }

    public static GameData LoadGame(int slotIndex)
    {
        string path = Application.persistentDataPath + string.Format(saveFileNameFormat, slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        return null;
    }

    public static void DeleteSave(int slotIndex)
    {
        string path = Application.persistentDataPath + string.Format(saveFileNameFormat, slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static List<GameData> GetAllSaveData()
    {
        List<GameData> allData = new List<GameData>();
        for (int i = 0; i < maxSaveSlots; i++)
        {
            allData.Add(LoadGame(i));
        }
        return allData;
    }

    public static bool SaveFileExists(int slotIndex)
    {
        string path = Application.persistentDataPath + string.Format(saveFileNameFormat, slotIndex);
        return File.Exists(path);
    }

    public static bool DoesAnySaveFileExist()
    {
        for (int i = 0; i < maxSaveSlots; i++)
        {
            if (SaveFileExists(i))
            {
                return true;
            }
        }
        return false;
    }
}