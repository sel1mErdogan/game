using UnityEngine;
using System.Collections.Generic;

public class UnitProductionUI : MonoBehaviour
{
    public static UnitProductionUI Instance { get; private set; }

    [Header("UI Referansları")]
    [SerializeField] private GameObject productionPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject unitButtonPrefab;

    [Header("Bağlantılar")]
    [Tooltip("Menüsü açılacak olan ana üs (ColonyBase)")]
    [SerializeField] private ColonyBase mainColonyBase; // YENİ EKLENDİ

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        productionPanel.SetActive(false);
    }

    // Artık bu fonksiyonu ana UI butonu çağıracak
    public void ToggleProductionMenu()
    {
        bool isActive = !productionPanel.activeSelf;
        productionPanel.SetActive(isActive);

        if (isActive)
        {
            PopulateProductionButtons();
        }
    }

    private void PopulateProductionButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        if (mainColonyBase == null)
        {
            Debug.LogError("UnitProductionUI'a hangi üssü açacağı söylenmemiş!");
            return;
        }

        List<UnitData> units = mainColonyBase.GetProducibleUnits();
        foreach (var unit in units)
        {
            GameObject buttonObj = Instantiate(unitButtonPrefab, buttonContainer);
            UnitButtonUI buttonUI = buttonObj.GetComponent<UnitButtonUI>();
            if (buttonUI != null)
            {
                buttonUI.Setup(unit, mainColonyBase);
            }
        }
    }
}