using UnityEngine;
using UnityEngine.UI;
using TMPro; // Eğer TextMeshPro kullanıyorsan bu satır kalsın

public class BuildingButtonUI : MonoBehaviour
{
    [SerializeField] private Image buildingIcon; // Butonun üzerindeki resim
    [SerializeField] private TextMeshProUGUI buildingNameText; // Butonun altındaki yazı

    private BuildingData assignedBuilding;

    // BuildMenuUI bu fonksiyonu çağırarak butonu ayarlar
    public void Setup(BuildingData buildingData)
    {
        assignedBuilding = buildingData;
        
        buildingIcon.sprite = assignedBuilding.buildingIcon;
        buildingNameText.text = assignedBuilding.buildingName;
    }

    // Bu fonksiyonu butonun OnClick event'ine bağlayacağız
    public void OnButtonClick()
    {
        if (assignedBuilding != null)
        {
            // BuildingManager'a inşaat modunu başlatmasını söyle
            BuildingManager.Instance.StartPlacementMode(assignedBuilding);
        }
    }
}