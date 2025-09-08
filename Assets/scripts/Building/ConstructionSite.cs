using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    // DEĞİŞİKLİK: Artık BuildingData'nın tamamını değil, sadece adını tutacağız.
    private string buildingName; 
    private float buildTime;

    private float currentBuildTime = 0f;
    private MasterBeetleAI assignedBuilder;

    // GameManager'ın bu bilgileri dışarıdan okuyabilmesi için
    public string GetBuildingName() { return buildingName; }
    public float GetCurrentBuildTime() { return currentBuildTime; }

    // Bir inşaat alanı oluşturulduğunda bu fonksiyon çağrılır
    public void Initialize(BuildingData data)
    {
        buildingName = data.buildingName;
        buildTime = data.buildTime;
    }

    // Oyunu yüklerken bu fonksiyon çağrılır
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
    
    // Bir MasterBeetle gelip inşaata başladığında bu çağrılır
    public void StartConstructing(MasterBeetleAI builder)
    {
        assignedBuilder = builder;
    }

    // İnşaat tamamlandığında
    private void CompleteConstruction()
    {
        // Binanın tam verisini veritabanından adıyla bul
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