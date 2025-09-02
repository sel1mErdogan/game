using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KingdomBug;
using System.Collections.Generic;

public class UpgradeDetailPanel : MonoBehaviour
{
    [Header("Genel Panel")]
    [SerializeField] private GameObject mainPanel; // Detay panelinin kendisi

    [Header("Böcek Bilgileri")]
    [SerializeField] private TextMeshProUGUI beetleNameText;

    [Header("Talep Gösterme Grubu")]
    [SerializeField] private GameObject requestGroup; // "Böcek bunu istiyor" grubu
    [SerializeField] private TextMeshProUGUI requestInfoText;
    [SerializeField] private Button approveRequestButton;
    [SerializeField] private Button denyRequestButton;

    [Header("Tümünü Listeleme Grubu")]
    [SerializeField] private GameObject allUpgradesGroup; // "Sen seç" grubu
    [SerializeField] private Transform allUpgradesContentParent; // Butonların oluşturulacağı yer
    [SerializeField] private GameObject upgradeChoiceButtonPrefab; // Seçim butonları için prefab

    private Beetle currentBeetle;

    private void Awake()
    {
        // Başlangıçta panel kapalı olsun.
        mainPanel.SetActive(false);
    }

    /// <summary>
    /// Bu fonksiyon, seçilen böceğe göre paneli doldurur.
    /// </summary>
    public void ShowPanelForBeetle(Beetle beetle)
    {
        currentBeetle = beetle;
        mainPanel.SetActive(true);
        beetleNameText.text = beetle.name;

        var requester = beetle.GetComponent<BeetleUpgradeRequester>();

        // Böceğin bir talebi var mı?
        if (requester != null && requester.HasRequest)
        {
            // Evet, talebi var. O zaman "Talep Gösterme" arayüzünü aç.
            ShowRequestView(requester.GetRequestedUpgrade());
        }
        else
        {
            // Hayır, talebi yok. O zaman oyuncunun seçebileceği "Tümünü Listeleme" arayüzünü aç.
            ShowAllUpgradesView();
        }
    }

    private void ShowRequestView(UpgradeData requestedUpgrade)
    {
        requestGroup.SetActive(true);
        allUpgradesGroup.SetActive(false);

        requestInfoText.text = $"Bu böcek bir '{requestedUpgrade.upgradeName}' olmak istiyor. Onaylıyor musun?";

        approveRequestButton.onClick.RemoveAllListeners();
        approveRequestButton.onClick.AddListener(() => {
            // Onayla ve paneli kapat
            UpgradeManager.Instance.PurchaseUpgrade(requestedUpgrade, currentBeetle);
            currentBeetle.GetComponent<BeetleUpgradeRequester>().ClearRequest();
            mainPanel.SetActive(false);
        });

        denyRequestButton.onClick.RemoveAllListeners();
        denyRequestButton.onClick.AddListener(() => {
            // Talebi reddet ve oyuncunun seçebileceği diğer tüm geliştirmeleri göster.
            // Bu, senin anlattığın o "reddedersem diğerleri çıksın" mantığı!
            currentBeetle.GetComponent<BeetleUpgradeRequester>().ClearRequest();
            ShowAllUpgradesView();
        });
    }

    private void ShowAllUpgradesView()
    {
        requestGroup.SetActive(false);
        allUpgradesGroup.SetActive(true);

        foreach (Transform child in allUpgradesContentParent)
        {
            Destroy(child.gameObject);
        }

        var requester = currentBeetle.GetComponent<BeetleUpgradeRequester>();
        if (requester == null) return;

        // Böceğin alabileceği tüm geliştirmeleri listele
        foreach (var upgrade in requester.possibleUpgrades)
        {
            // Zaten sahip olduğu geliştirmeleri listeleme
            if (currentBeetle.HasUpgrade(upgrade)) continue;

            GameObject buttonObj = Instantiate(upgradeChoiceButtonPrefab, allUpgradesContentParent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.upgradeName;
            
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => {
                // Seçilen geliştirmeyi satın al ve paneli kapat
                UpgradeManager.Instance.PurchaseUpgrade(upgrade, currentBeetle);
                mainPanel.SetActive(false);
            });
        }
    }
}