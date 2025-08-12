using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    private BuildingData buildingToBuild;
    private float currentBuildTime = 0f;
    private bool isBuilderPresent = false;
    private MasterBeetleAI assignedBuilder; // SINIF ADI DÜZELTİLDİ

    public void Initialize(BuildingData data)
    {
        buildingToBuild = data;
    }

    private void Update()
    {
        if (isBuilderPresent)
        {
            currentBuildTime += Time.deltaTime;
            if (currentBuildTime >= buildingToBuild.buildTime)
            {
                CompleteConstruction();
            }
        }
    }

    // SINIF ADI DÜZELTİLDİ
    public void StartConstructing(MasterBeetleAI builder)
    {
        assignedBuilder = builder;
        isBuilderPresent = true;
    }

    private void CompleteConstruction()
    {
        if (buildingToBuild.finishedBuildingPrefab != null)
        {
            Instantiate(buildingToBuild.finishedBuildingPrefab, transform.position, transform.rotation);
        }
        
        if (assignedBuilder != null)
        {
            assignedBuilder.TaskFinished();
        }
        
        Destroy(gameObject);
    }
}