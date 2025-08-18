using UnityEngine;

public class CanSistemi : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] private int maxCan = 100;
    private int mevcutCan;

    [Header("Nüfus Ayarları")]
    [Tooltip("Bu birim, koloni nüfusuna dahil mi? (Böcekler için işaretle)")]
    [SerializeField] private bool isColonyUnit = false;
    [Tooltip("Nüfusta kapladığı yer.")]
    [SerializeField] private int populationCost = 1;

    [Header("UI Ayarları")]
    [Tooltip("Bu birim için oluşturulacak Can Barı prefab'ı")]
    [SerializeField] private GameObject healthBarPrefab;
    [Tooltip("Can barının birimin ne kadar üzerinde duracağı")]
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    private HealthBarUI healthBar;

    private void Start()
    {
        mevcutCan = maxCan;

        if (healthBarPrefab != null)
        {
            GameObject healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity, transform);
            healthBar = healthBarInstance.GetComponent<HealthBarUI>();
        }
    }

    public void HasarAl(int hasarMiktari)
    {
        if (mevcutCan <= 0) return;

        mevcutCan -= hasarMiktari;

        if (mevcutCan < 0) mevcutCan = 0;

        if (healthBar != null)
        {
            healthBar.UpdateBar((float)mevcutCan / maxCan);
        }

        if (mevcutCan <= 0)
        {
            Ol();
        }
    }

    private void Ol()
    {
        if (isColonyUnit && ColonyManager.Instance != null)
        {
            ColonyManager.Instance.RemovePopulation(populationCost);
        }
        
        Destroy(gameObject);
    }
}