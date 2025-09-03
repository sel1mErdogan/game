using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KingdomBug;
using System.Collections.Generic;

public class UpgradeDetailPanel : MonoBehaviour
{
    [Header("Genel Panel")]
    [SerializeField] private GameObject mainPanel;

    [Header("Böcek Bilgileri")]
    [SerializeField] private TextMeshProUGUI beetleNameText;

    [Header("Talep Gösterme Grubu")]
    [SerializeField] private GameObject requestGroup;
    [SerializeField] private TextMeshProUGUI requestInfoText;
    [SerializeField] private Button approveRequestButton;
    [SerializeField] private Button denyRequestButton;

    [Header("Tümünü Listeleme Grubu")]
    [SerializeField] private GameObject allUpgradesGroup;
    [SerializeField] private Transform allUpgradesContentParent;
    [SerializeField] private GameObject upgradeChoiceButtonPrefab;
    
    [Header("Seçili Geliştirme Detay Grubu")]
    [SerializeField] private GameObject detailsSubPanel;
    [SerializeField] private Image detailIconImage;
    [SerializeField] private TextMeshProUGUI detailNameText;
    [SerializeField] private TextMeshProUGUI detailDescriptionText;
    [SerializeField] private TextMeshProUGUI detailCostText;
    [SerializeField] private Button buyButton;
    
    [Header("Mevcut Geliştirmeler Grubu")]
    [SerializeField] private GameObject ownedUpgradesGroup;
    [SerializeField] private Transform ownedUpgradesContentParent;
    [SerializeField] private GameObject ownedUpgradeLabelPrefab;

    private Beetle currentBeetle;
    private UpgradeData currentlySelectedUpgrade;

    private void Awake()
    {
        mainPanel.SetActive(false);
    }

    // ARTIK SADECE "BEETLE" ALIYOR, BAŞKA BİR ŞEY DEĞİL!
    public void ShowPanelForBeetle(Beetle beetle)
    {
        currentBeetle = beetle;
        mainPanel.SetActive(true);
        beetleNameText.text = beetle.name;

        // BU FONKSİYONU ÇAĞIRIYORUZ
        PopulateOwnedUpgrades();

        var requester = beetle.GetComponent<BeetleUpgradeRequester>();
        if (requester != null && requester.HasRequest)
        {
            ShowRequestView(requester);
        }
        else
        {
            ShowAllUpgradesListView();
        }
    }

    private void ShowRequestView(BeetleUpgradeRequester requester)
    {
        requestGroup.SetActive(true);
        allUpgradesGroup.SetActive(false);
        detailsSubPanel.SetActive(false);
        
        UpgradeData requestedUpgrade = requester.GetRequestedUpgrade();
        requestInfoText.text = $"Bu böcek bir '{requestedUpgrade.upgradeName}' olmak istiyor. Onaylıyor musun?";

        approveRequestButton.onClick.AddListener(() => {
            if (UpgradeManager.Instance.PurchaseUpgrade(requestedUpgrade, currentBeetle))
            {
                requester.ClearRequest();
                // Paneli direkt kapatmak yerine, son haliyle yenileyelim
                ShowPanelForBeetle(currentBeetle); 
            }
        });

        denyRequestButton.onClick.RemoveAllListeners();
        denyRequestButton.onClick.AddListener(() => {
            requester.ClearRequest();
            ShowAllUpgradesListView();
        });
    }

    private void ShowAllUpgradesListView()
    {
        requestGroup.SetActive(false);
        allUpgradesGroup.SetActive(true);
        detailsSubPanel.SetActive(false);

        foreach (Transform child in allUpgradesContentParent) { Destroy(child.gameObject); }
        
        var requester = currentBeetle.GetComponent<BeetleUpgradeRequester>();
        if (requester == null) return;
        
        // BİLGİYİ DOĞRUDAN BÖCEĞİN KENDİSİNDEN ALIYORUZ! UPGRADEDB YOK!
        var possibleUpgrades = requester.possibleUpgrades;

        foreach (var upgrade in possibleUpgrades)
        {
            if (currentBeetle.HasUpgrade(upgrade)) continue;

            GameObject buttonObj = Instantiate(upgradeChoiceButtonPrefab, allUpgradesContentParent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.upgradeName;
            Button button = buttonObj.GetComponent<Button>();
            
            button.onClick.AddListener(() => ShowUpgradeDetails(upgrade));
        }
    }

    private void ShowUpgradeDetails(UpgradeData upgrade)
    {
        currentlySelectedUpgrade = upgrade;
        detailsSubPanel.SetActive(true);

        detailIconImage.sprite = upgrade.icon;
        detailNameText.text = upgrade.upgradeName;
        detailDescriptionText.text = upgrade.description;

        string costString = "Maliyet:\n";
        foreach (var cost in upgrade.cost) { costString += $"{cost.amount} {cost.resource.itemName}\n"; }
        detailCostText.text = costString;
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(ConfirmPurchase);
    }
    
    private void ConfirmPurchase()
    {
        if (UpgradeManager.Instance.PurchaseUpgrade(currentlySelectedUpgrade, currentBeetle))
        {
            // Satın alım başarılı, paneli en baştan yenileyerek son durumu gösterelim.
            ShowPanelForBeetle(currentBeetle);
        }
    }

    private void PopulateOwnedUpgrades()
    {
        // Önce listedeki eski etiketleri temizle
        foreach (Transform child in ownedUpgradesContentParent) { Destroy(child.gameObject); }

        // Böceğin sahip olduğu geliştirmelerin listesini al
        var ownedUpgrades = currentBeetle.individualUpgrades;

        // Eğer böceğin hiç geliştirmesi yoksa, bu bölümü tamamen gizle
        if (ownedUpgrades == null || ownedUpgrades.Count == 0)
        {
            ownedUpgradesGroup.SetActive(false);
            return;
        }

        // Eğer varsa, bölümü göster ve içini doldur
        ownedUpgradesGroup.SetActive(true);
        foreach (var ownedUpgrade in ownedUpgrades)
        {
            GameObject labelObj = Instantiate(ownedUpgradeLabelPrefab, ownedUpgradesContentParent);
            labelObj.GetComponent<TextMeshProUGUI>().text = "- " + ownedUpgrade.upgradeName;
        }
    }
}