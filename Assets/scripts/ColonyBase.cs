using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using KingdomBug;

public class ColonyBase : MonoBehaviour
{
    [Header("Üretim Ayarları")]
    [Tooltip("Yeni böceklerin doğacağı nokta")]
    [SerializeField] private Transform spawnPoint; 
    [Tooltip("Bu üste üretilebilecek birimlerin UnitData listesi")]
    [SerializeField] private List<UnitData> producibleUnits;

    public List<UnitData> GetProducibleUnits()
    {
        return producibleUnits;
    }

    public void StartTrainingUnit(UnitData unitToTrain)
    {
        if (!ColonyManager.Instance.AddPopulation(unitToTrain.populationCost)) return;

        if (!InventoryManager.Instance.SpendResources(unitToTrain.productionCost))
        {
            ColonyManager.Instance.RemovePopulation(unitToTrain.populationCost);
            return;
        }

        StartCoroutine(TrainUnitCoroutine(unitToTrain));
    }

    private IEnumerator TrainUnitCoroutine(UnitData unitData)
    {
        yield return new WaitForSeconds(unitData.productionTime);
        if (unitData.unitPrefab != null && spawnPoint != null)
        {
            Instantiate(unitData.unitPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    [Header("Kaynak Bırakma Ayarları")]
    [SerializeField] private float depositRadius = 5f;

    private void Awake()
    {
        if (GetComponent<SphereCollider>() == null)
        {
            SphereCollider triggerCollider = gameObject.AddComponent<SphereCollider>();
            triggerCollider.radius = depositRadius;
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Beetle beetle = other.GetComponent<Beetle>();
        if (beetle != null && beetle.HasItems())
        {
            Dictionary<ItemData, int> items = beetle.EmptyInventory();
            foreach (var item in items)
            {
                InventoryManager.Instance.AddItem(item.Key, item.Value);
            }
        }
    }
}