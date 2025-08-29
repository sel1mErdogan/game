using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [Header("Böcek Türü Geliştirme Listeleri")]
    [Tooltip("İşçi Böcek için oluşturduğun tüm UpgradeData'ları buraya sürükle.")]
    [SerializeField] private List<UpgradeData> workerUpgrades;
    [Tooltip("Keşifçi Böcek için oluşturduğun tüm UpgradeData'ları buraya sürükle.")]
    [SerializeField] private List<UpgradeData> explorerUpgrades;
    [Tooltip("Savaşçı Böcek için oluşturduğun tüm UpgradeData'ları buraya sürükle.")]
    [SerializeField] private List<UpgradeData> warriorUpgrades;
    [Tooltip("Usta Böcek için oluşturduğun tüm UpgradeData'ları buraya sürükle.")]
    [SerializeField] private List<UpgradeData> masterUpgrades;

    [Header("Arayüz Referansları")]
    [Tooltip("ScrollView'in içindeki 'Content' objesi.")]
    [SerializeField] private Transform contentParent;
    [Tooltip("Proje klasöründeki UpgradeButton_Prefab'ı.")]
    [SerializeField] private GameObject upgradeButtonPrefab;

    [Header("Detay Paneli Elemanları")]
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailCost;
    [SerializeField] private Button buyButton;

    private List<UpgradeButtonUI> currentButtons = new List<UpgradeButtonUI>();
    private UpgradeButtonUI selectedUpgradeButton;

    private void Start()
    {
        // Başlangıçta paneli gizle ve işçi sekmesini varsayılan olarak göster
        detailsPanel.SetActive(false);
        ShowUpgradesFor(workerUpgrades);
        buyButton.onClick.AddListener(PurchaseSelectedUpgrade);
    }

    // Bu dört fonksiyonu, sekme butonlarından çağıracağız.
    public void ShowWorkerUpgrades() => ShowUpgradesFor(workerUpgrades);
    public void ShowExplorerUpgrades() => ShowUpgradesFor(explorerUpgrades);
    public void ShowWarriorUpgrades() => ShowUpgradesFor(warriorUpgrades);
    public void ShowMasterUpgrades() => ShowUpgradesFor(masterUpgrades);

    private void ShowUpgradesFor(List<UpgradeData> upgrades)
    {
        // Önceki butonları temizle
        foreach (Transform child in contentParent) Destroy(child.gameObject);
        currentButtons.Clear();
        detailsPanel.SetActive(false); // Sekme değiştiğinde detay panelini gizle

        // Listeden yeni butonları oluştur
        foreach (var upgrade in upgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, contentParent);
            UpgradeButtonUI buttonUI = buttonObj.GetComponent<UpgradeButtonUI>();
            buttonUI.Setup(upgrade, this);
            currentButtons.Add(buttonUI);
        }
    }

    // Bir geliştirme butonuna tıklandığında bu fonksiyon çağrılır
    public void SelectUpgrade(UpgradeButtonUI selectedButton)
    {
        selectedUpgradeButton = selectedButton;
        UpgradeData data = selectedButton.GetUpgradeData();
        
        detailsPanel.SetActive(true);
        detailIcon.sprite = data.icon;
        detailName.text = data.upgradeName;
        detailDescription.text = data.description;
        
        // Maliyeti formatlayıp yazdır
        string costString = "Maliyet:\n";
        foreach(var cost in data.cost)
        {
            costString += $"{cost.amount} {cost.resource.itemName}\n";
        }
        detailCost.text = costString;

        buyButton.interactable = UpgradeManager.Instance.CanPurchaseUpgrade(data) && !UpgradeManager.Instance.IsUpgradePurchased(data);
    }

    private void PurchaseSelectedUpgrade()
    {
        if(selectedUpgradeButton != null)
        {
            UpgradeManager.Instance.PurchaseUpgrade(selectedUpgradeButton.GetUpgradeData());
            
            // Satın alımdan sonra tüm butonların görsellerini güncelle
            // (örn: bir sonraki geliştirme artık alınabilir hale gelebilir)
            foreach(var button in currentButtons)
            {
                button.UpdateVisuals();
            }
            
            // Detay panelindeki butonu da güncelle
            SelectUpgrade(selectedUpgradeButton);
        }
    }
}