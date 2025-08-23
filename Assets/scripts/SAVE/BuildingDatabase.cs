// --- BuildingDatabase.cs ---
using UnityEngine;
using System.Collections.Generic;

public class BuildingDatabase : MonoBehaviour
{
    public static BuildingDatabase Instance { get; private set; }

    [Tooltip("Projedeki TÜM BuildingData ScriptableObject'larını buraya sürükleyin.")]
    [SerializeField] private List<BuildingData> allBuildings;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Verilen isme göre BuildingData'yı listeden bulup döndürür.
    public BuildingData FindBuildingByName(string name)
    {
        return allBuildings.Find(building => building.buildingName == name);
    }
}