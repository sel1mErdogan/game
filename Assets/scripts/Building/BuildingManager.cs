using UnityEngine;
using UnityEngine.InputSystem;
using InventorySystem;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [SerializeField] private LayerMask groundLayer;
    private BuildingData selectedBuilding;
    private GameObject placementGhost;
    private bool isInPlacementMode = false;

    // --- YENİ EKLENDİ: Titremeyi önlemek için ---
    private float placementYOffset; // Yükseklik farkını hafızada tutacak değişken.

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

    public void StartPlacementMode(BuildingData building)
    {
        if (InventoryManager.Instance.SpendResources(building.requiredMaterials))
        {
            selectedBuilding = building;
            placementGhost = Instantiate(building.placementGhostPrefab);
            isInPlacementMode = true;

            // --- YENİ EKLENDİ: Yükseklik farkını SADECE BİR KERE HESAPLA ---
            // Hayalet objenin pivot noktası ile en alt noktası arasındaki mesafeyi bul ve hafızaya al.
            Collider ghostCollider = placementGhost.GetComponentInChildren<Collider>();
            if (ghostCollider != null)
            {
                placementYOffset = placementGhost.transform.position.y - ghostCollider.bounds.min.y;
            }
            else
            {
                placementYOffset = 0f; // Collider yoksa, fark sıfırdır.
            }
        }
        else
        {
            Debug.LogWarning("İnşaat için yeterli malzeme yok!");
        }
    }

    private void HandlePlacementMode()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            // --- YENİ EKLENDİ: Pozisyonu, hafızadaki sabit değere göre ayarla ---
            Vector3 position = new Vector3(hit.point.x, hit.point.y + placementYOffset, hit.point.z);
            placementGhost.transform.position = position;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceBuilding();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    // BuildingManager.cs içindeki PlaceBuilding fonksiyonunun son hali
    private void PlaceBuilding()
    {
        GameObject constructionSiteObj = Instantiate(selectedBuilding.constructionSitePrefab, placementGhost.transform.position, placementGhost.transform.rotation);
        ConstructionSite siteScript = constructionSiteObj.GetComponent<ConstructionSite>();
        if (siteScript != null)
        {
            // Initialize fonksiyonuna tam BuildingData'yı gönderiyoruz
            siteScript.Initialize(selectedBuilding); 
        }
        
        AssignTaskToMasterBeetle(constructionSiteObj.transform);
        CleanUpPlacementMode();
    }

    private void AssignTaskToMasterBeetle(Transform siteTransform)
    {
        MasterBeetleAI[] allMasters = FindObjectsOfType<MasterBeetleAI>();
        MasterBeetleAI closestAvailableMaster = null;
        float minDistance = float.MaxValue;

        foreach (var master in allMasters)
        {
            if (master.IsAvailable())
            {
                float distance = Vector3.Distance(master.transform.position, siteTransform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestAvailableMaster = master;
                }
            }
        }

        if (closestAvailableMaster != null)
        {
            closestAvailableMaster.AssignBuildTask(siteTransform);
        }
        else
        {
            Debug.LogWarning("Boşta Usta Böcek bulunamadı!");
        }
    }

    private void CancelPlacement()
    {
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