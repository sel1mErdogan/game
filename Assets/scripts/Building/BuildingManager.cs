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

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update()
    {
        if (!isInPlacementMode || Mouse.current == null)
        {
            return;
        }
        
        HandlePlacementMode();
    }

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
            // Bu önemli bir uyarı olduğu için kalabilir.
            Debug.LogWarning("İnşaat için yeterli malzeme yok!");
        }
    }

    private void HandlePlacementMode()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            // --- YÜKSEKLİK DÜZELTMESİ BURADA ---
            Vector3 position = hit.point;

            // Hayalet objenin yüksekliğini alıp yarısını ekleyerek objeyi zeminin üzerine çıkar.
            // Bu, objenin yere gömülmesini engeller.
            Renderer ghostRenderer = placementGhost.GetComponentInChildren<Renderer>();
            if (ghostRenderer != null)
            {
                position.y += ghostRenderer.bounds.size.y / 2;
            }

            placementGhost.transform.position = position;
        }
        // Test amaçlı Debug.Log mesajları kaldırıldı.

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceBuilding();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
        }
    }

    private void PlaceBuilding()
    {
        // İnşaat alanını, yüksekliği zaten düzeltilmiş olan hayaletin pozisyonuna kur.
        GameObject constructionSiteObj = Instantiate(selectedBuilding.constructionSitePrefab, placementGhost.transform.position, placementGhost.transform.rotation);
        
        ConstructionSite siteScript = constructionSiteObj.GetComponent<ConstructionSite>();
        if (siteScript != null)
        {
            siteScript.Initialize(selectedBuilding);
        }
        
        CleanUpPlacementMode();
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