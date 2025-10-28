using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    private string buildingName;
    private float buildTime;

    private float currentBuildTime = 0f;
    private MasterBeetleAI assignedBuilder;

    // --- BU FONKSİYONU EKLE ---
    /// <summary>
    /// Bu inşaat alanına bir böcek atanmış mı?
    /// </summary>
    public bool IsAssigned()
    {
        return assignedBuilder != null;
    }
    // --- BİTTİ ---

    public string GetBuildingName() { return buildingName; }
    public float GetCurrentBuildTime() { return currentBuildTime; }

    public void Initialize(BuildingData data)
    {
        buildingName = data.buildingName;
        buildTime = data.buildTime;
    }

    public void LoadProgress(string name, float time, float maxTime)
    {
        buildingName = name;
        currentBuildTime = time;
        buildTime = maxTime;
    }

    private void Update()
    {
        if (assignedBuilder != null)
        {
            currentBuildTime += Time.deltaTime;
            if (currentBuildTime >= buildTime)
            {
                CompleteConstruction();
            }
        }
    }

    public void StartConstructing(MasterBeetleAI builder)
    {
        assignedBuilder = builder;
    }

    private void CompleteConstruction()
    {
        BuildingData data = BuildingDatabase.Instance.FindBuildingByName(buildingName);
        if (data != null && data.finishedBuildingPrefab != null)
        {
            Instantiate(data.finishedBuildingPrefab, transform.position, transform.rotation);
        }

        if (assignedBuilder != null)
        {
            assignedBuilder.TaskFinished();
        }

        Destroy(gameObject);
    }
}