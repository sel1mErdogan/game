// --- ConstructionSite.cs (Güncellenmiş Hali) ---
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    private BuildingData buildingToBuild;
    private float currentBuildTime = 0f;
    private bool isBuilderPresent = false;
    private MasterBeetleAI assignedBuilder;

    // YENİ EKLENDİ: GameManager'ın bu bilgileri okuyabilmesi için
    public BuildingData GetBuildingData() { return buildingToBuild; }
    public float GetCurrentBuildTime() { return currentBuildTime; }

    public void Initialize(BuildingData data)
    {
        buildingToBuild = data;
    }

    // YENİ EKLENDİ: Kayıttan yüklenen ilerlemeyi ayarlamak için
    public void LoadProgress(BuildingData data, float time)
    {
        buildingToBuild = data;
        currentBuildTime = time;
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
    
    // Diğer fonksiyonlar (StartConstructing, CompleteConstruction) aynı kalacak...
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