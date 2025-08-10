using UnityEngine;
using InventorySystem; // InventoryManager'a erişim için
using System.Collections.Generic; // List kullanmak için

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [SerializeField] private LayerMask groundLayer; // İnşaatın yapılabileceği zemin katmanı
    private BuildingData selectedBuilding;
    private GameObject placementGhost;
    private bool isInPlacementMode = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update()
    {
        if (isInPlacementMode)
        {
            HandlePlacementMode();
        }
    }

    // UI butonları bu fonksiyonu çağıracak
    public void StartPlacementMode(BuildingData building)
    {
        if (InventoryManager.Instance.SpendResources(building.requiredMaterials))
        {
            selectedBuilding = building;
            placementGhost = Instantiate(building.placementGhostPrefab);
            isInPlacementMode = true;
        }
        else
        {
            Debug.Log("İnşaat için yeterli malzeme yok!");
            // Burada oyuncuya bir uyarı sesi veya mesajı gösterebilirsin.
        }
    }

    private void HandlePlacementMode()
    {
        // Fare pozisyonunu dünyaya çevir
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            placementGhost.transform.position = hit.point;
        }

        // Sol tıklama ile inşaatı onayla
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }

        // Sağ tıklama ile iptal et
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void PlaceBuilding()
    {
        // İnşaat alanını oluştur
        GameObject constructionSiteObj = Instantiate(selectedBuilding.constructionSitePrefab, placementGhost.transform.position, placementGhost.transform.rotation);
        
        // ConstructionSite script'ine hangi binayı inşa ettiğini söyle
        ConstructionSite siteScript = constructionSiteObj.GetComponent<ConstructionSite>();
        if (siteScript != null)
        {
            siteScript.Initialize(selectedBuilding);
        }
        
        // TODO: Boşta bir Usta Böcek bul ve ona görev ver.
        // FindAvailableMasterBeetle().AssignBuildTask(constructionSiteObj.transform);

        CleanUpPlacementMode();
    }

    private void CancelPlacement()
    {
        // Harcanan kaynakları geri iade et
        InventoryManager.Instance.AddResources(selectedBuilding.requiredMaterials);
        CleanUpPlacementMode();
    }
    
    private void CleanUpPlacementMode()
    {
        isInPlacementMode = false;
        Destroy(placementGhost);
        selectedBuilding = null;
    }
}