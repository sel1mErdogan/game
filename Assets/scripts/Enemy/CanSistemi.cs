// --- CanSistemi.cs (Güncellenmiş Hali) ---
using UnityEngine;

public class CanSistemi : MonoBehaviour
{
    [Header("Can Ayarları")]
    [SerializeField] private int maxCan = 100;
    private int mevcutCan;

    // mevcutCan'ı dışarıdan okunabilir hale getirdik.
    public int MevcutCan { get { return mevcutCan; } }
    public int MaxCan { get { return maxCan; } }

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
        // Eğer mevcut can daha önce LoadHealth ile set edilmediyse, max candan başla.
        if (mevcutCan == 0)
        {
            mevcutCan = maxCan;
        }

        if (healthBarPrefab != null)
        {
            GameObject healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity, transform);
            healthBar = healthBarInstance.GetComponent<HealthBarUI>();
            UpdateHealthBar(); // Başlangıçta can barını güncelle
        }
    }

    public void HasarAl(int hasarMiktari)
    {
        if (mevcutCan <= 0) return;
        mevcutCan -= hasarMiktari;
        if (mevcutCan < 0) mevcutCan = 0;
        
        UpdateHealthBar();

        if (mevcutCan <= 0)
        {
            Ol();
        }
    }
    
    // YENİ EKLENDİ: Canı kayıttan yüklemek için.
    public void LoadHealth(int health)
    {
        mevcutCan = health;
        // Henüz Start çalışmadığı için can barını burada güncelleyemeyiz.
        // Start içinde halledilecek.
    }
    
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateBar((float)mevcutCan / maxCan);
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