using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    // ... Değişkenler aynı kalacak ...
    [Header("Böcek Türü Geliştirme Listeleri")]
    [SerializeField] private List<UpgradeData> workerUpgrades;
    [SerializeField] private List<UpgradeData> explorerUpgrades;
    [SerializeField] private List<UpgradeData> warriorUpgrades;
    [SerializeField] private List<UpgradeData> masterUpgrades;

    [Header("Arayüz Referansları")]
    [SerializeField] private Transform contentParent;
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


    // Start, Show...Upgrades, ShowUpgradesFor ve SelectUpgrade fonksiyonları aynı...

    private void PurchaseSelectedUpgrade()
    {
        if(selectedUpgradeButton != null)
        {
            // Satın alma işlemini dene
            bool success = UpgradeManager.Instance.PurchaseUpgrade(selectedUpgradeButton.GetUpgradeData());
            
            // SADECE satın alım başarılıysa arayüzü güncelle
            if (success)
            {
                // Tüm butonların görsellerini güncelle
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
    
    // Diğer tüm fonksiyonların tam hali (kopyala-yapıştır için)
    private void Start() { detailsPanel.SetActive(false); ShowUpgradesFor(workerUpgrades); buyButton.onClick.AddListener(PurchaseSelectedUpgrade); }
    public void ShowWorkerUpgrades() => ShowUpgradesFor(workerUpgrades);
    public void ShowExplorerUpgrades() => ShowUpgradesFor(explorerUpgrades);
    public void ShowWarriorUpgrades() => ShowUpgradesFor(warriorUpgrades);
    public void ShowMasterUpgrades() => ShowUpgradesFor(masterUpgrades);
    private void ShowUpgradesFor(List<UpgradeData> upgrades) { foreach (Transform child in contentParent) Destroy(child.gameObject); currentButtons.Clear(); detailsPanel.SetActive(false); foreach (var upgrade in upgrades) { GameObject buttonObj = Instantiate(upgradeButtonPrefab, contentParent); UpgradeButtonUI buttonUI = buttonObj.GetComponent<UpgradeButtonUI>(); buttonUI.Setup(upgrade, this); currentButtons.Add(buttonUI); } }
    public void SelectUpgrade(UpgradeButtonUI selectedButton) { selectedUpgradeButton = selectedButton; UpgradeData data = selectedButton.GetUpgradeData(); detailsPanel.SetActive(true); detailIcon.sprite = data.icon; detailName.text = data.upgradeName; detailDescription.text = data.description; string costString = "Maliyet:\n"; foreach(var cost in data.cost) { costString += $"{cost.amount} {cost.resource.itemName}\n"; } detailCost.text = costString; if (UpgradeManager.Instance.IsUpgradePurchased(data)) { buyButton.interactable = false; } else { buyButton.interactable = UpgradeManager.Instance.CanPurchaseUpgrade(data); } }
}