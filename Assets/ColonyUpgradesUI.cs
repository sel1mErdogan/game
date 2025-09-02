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

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
    
    // --- BİTTİ ---

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        allBeetles = FindObjectsOfType<Beetle>().ToList();
        FilterAndDisplayBugs(BeetleType.Worker);
    }

    private void FilterAndDisplayBugs(BeetleType type)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<Beetle> filteredBeetles = allBeetles.Where(b => b.GetBeetleType() == type).ToList();

        foreach (Beetle beetle in filteredBeetles)
        {
            GameObject buttonObj = Instantiate(böcekButonPrefab, contentParent);
            BugListButton bugButton = buttonObj.GetComponent<BugListButton>();
            bugButton.Setup(beetle, this);
        }
    }

    public void OnBugSelected(Beetle beetle)
    {
        Debug.Log(beetle.name + " seçildi! Detay paneli dolduruluyor...");
        detayPaneli.ShowPanelForBeetle(beetle);
    }
}