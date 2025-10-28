using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using KingdomBug;
using System.Linq;

public class ColonyUpgradesUI : MonoBehaviour
{
    [Header("Filtre Butonları")]
    [SerializeField] private Button isciFiltreButonu;
    [SerializeField] private Button savasciFiltreButonu;
    [SerializeField] private Button kesifciFiltreButonu;
    [SerializeField] private Button ustaFiltreButonu;

    [Header("Liste Ayarları")]
    [SerializeField] private GameObject böcekButonPrefab;
    [SerializeField] private Transform contentParent;
    [Header("Detay Paneli")]
    [SerializeField] private UpgradeDetailPanel detayPaneli;

    private List<Beetle> allBeetles = new List<Beetle>();

    private void Start()
    {
        isciFiltreButonu.onClick.AddListener(() => FilterAndDisplayBugs(BeetleType.Worker));
        savasciFiltreButonu.onClick.AddListener(() => FilterAndDisplayBugs(BeetleType.Warrior));
        kesifciFiltreButonu.onClick.AddListener(() => FilterAndDisplayBugs(BeetleType.Explorer));
        ustaFiltreButonu.onClick.AddListener(() => FilterAndDisplayBugs(BeetleType.Master));

        // Panelin başlangıçta kapalı olduğundan emin ol
        gameObject.SetActive(false);
    }

    // --- YENİ EKLENEN FONKSİYONLAR ---
    
    /// <summary>
    /// Bu fonksiyon, panelin o anki durumuna bakarak onu açar veya kapatır.
    /// </summary>
    public void TogglePanel()
    {
        // gameObject.activeSelf, bu panelin o an aktif olup olmadığını kontrol eder (true/false)
        bool isActive = gameObject.activeSelf;

        // Eğer panel açıksa kapat, kapalıysa aç.
        if (isActive)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

   
    
    // --- BİTTİ ---

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        allBeetles = FindObjectsOfType<Beetle>().ToList();
        FilterAndDisplayBugs(BeetleType.Worker);
    }
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // ColonyUpgradesUI.cs script'inin içindeki fonksiyon

    private void FilterAndDisplayBugs(BeetleType type)
    {
        // --- EKLENECEK OLAN TEK SATIR BURASI ---
        detayPaneli.gameObject.SetActive(false); // Sekme değiştiğinde detay panelini kapat.
        // --- ---

        // Önce listedeki eski butonları temizle
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Geri kalan kodlar aynı...
        List<Beetle> filteredBeetles = ColonyManager.Instance.AllBeetles.Where(b => b.GetBeetleType() == type).ToList();

        foreach (Beetle beetle in filteredBeetles)
        {
            GameObject buttonObj = Instantiate(böcekButonPrefab, contentParent);
            BugListButton bugButton = buttonObj.GetComponent<BugListButton>();
            if (bugButton != null)
            {
                bugButton.Setup(beetle, this);
            }
        }
    }

    // ColonyUpgradesUI.cs içindeki OnBugSelected fonksiyonunun DOĞRU hali

    public void OnBugSelected(Beetle beetle)
    {
        Debug.Log(beetle.name + " seçildi! Detay paneli dolduruluyor...");
        
        // Detay Paneline sadece ve sadece seçilen böceği gönderiyoruz.
        // Başka hiçbir bilgiye gerek yok.
        detayPaneli.ShowPanelForBeetle(beetle);
    }
}