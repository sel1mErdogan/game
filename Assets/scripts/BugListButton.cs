using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KingdomBug;

public class BugListButton : MonoBehaviour
{
    [Header("Bileşen Referansları")]
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject notificationIcon; // Talep olduğunda parlayacak yeşil ikon
    [SerializeField] private Button mainButton;

    private Beetle assignedBeetle;
    private ColonyUpgradesUI mainPanel;

    /// <summary>
    /// Bu butonu, belirli bir böcek için ayarlar.
    /// </summary>
    public void Setup(Beetle beetle, ColonyUpgradesUI panel)
    {
        this.assignedBeetle = beetle;
        this.mainPanel = panel;

        // Böceğin seviyesini ve adını yazdır
        var experience = beetle.GetComponent<BeetleExperience>();
        infoText.text = $"{beetle.name} (Seviye {experience.level})";

        // Böceğin bir talebi var mı diye kontrol et
        var requester = beetle.GetComponent<BeetleUpgradeRequester>();
        if (requester != null && requester.HasRequest)
        {
            // Eğer talebi varsa, yeşil ikonu göster!
            notificationIcon.SetActive(true);
        }
        else
        {
            notificationIcon.SetActive(false);
        }

        // Butona tıklandığında ana panele haber ver
        mainButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // Ana panele "Ben seçildim, benim temsil ettiğim böcek bu:" diye haber ver.
        mainPanel.OnBugSelected(assignedBeetle);
    }
}