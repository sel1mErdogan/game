using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    private BuildingData buildingToBuild;
    private float currentBuildTime = 0f;
    private bool isInitialized = false;

    // BuildingManager bu fonksiyonu çağırarak inşaatı başlatır
    public void Initialize(BuildingData data)
    {
        buildingToBuild = data;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        currentBuildTime += Time.deltaTime;

        // İnşaat süresi dolduğunda
        if (currentBuildTime >= buildingToBuild.buildTime)
        {
            CompleteConstruction();
        }
    }

    private void CompleteConstruction()
    {
        // Tamamlanmış binayı oluştur
        if (buildingToBuild.finishedBuildingPrefab != null)
        {
            Instantiate(buildingToBuild.finishedBuildingPrefab, transform.position, transform.rotation);
        }
        
        // TODO: Bu inşaatta çalışan Usta Böceği serbest bırak.
        // assignedMasterBeetle.TaskFinished();
        
        // İnşaat alanını yok et
        Destroy(gameObject);
    }
}