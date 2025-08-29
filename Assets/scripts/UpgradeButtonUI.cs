using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button purchaseButton;

    private UpgradeData assignedUpgrade;
    private UpgradeUI parentUI;

    public void Setup(UpgradeData upgrade, UpgradeUI ui)
    {
        assignedUpgrade = upgrade;
        parentUI = ui;

        iconImage.sprite = upgrade.icon;
        nameText.text = upgrade.upgradeName;
        
        purchaseButton.onClick.AddListener(OnButtonClicked);
        
        UpdateVisuals();
    }

    private void OnButtonClicked()
    {
        // Butona tıklandığında, ana UI'a "ben seçildim" diye haber ver.
        parentUI.SelectUpgrade(this);
    }

    public void UpdateVisuals()
    {
        bool isPurchased = UpgradeManager.Instance.IsUpgradePurchased(assignedUpgrade);
        bool canPurchase = UpgradeManager.Instance.CanPurchaseUpgrade(assignedUpgrade);

        // Satın alındıysa yeşil, alınabilirse normal, alınamazsa kırmızı/gri yap.
        if (isPurchased)
        {
            GetComponent<Image>().color = Color.green;
            purchaseButton.interactable = false;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
            purchaseButton.interactable = canPurchase;
        }
    }
    
    public UpgradeData GetUpgradeData()
    {
        return assignedUpgrade;
    }
}