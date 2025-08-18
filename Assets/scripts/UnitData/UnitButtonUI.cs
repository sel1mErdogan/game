using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitButtonUI : MonoBehaviour
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private TextMeshProUGUI unitNameText;

    private UnitData assignedUnit;
    private ColonyBase assignedBase;

    // UnitProductionUI bu fonksiyonu çağırarak butonu ayarlar
    public void Setup(UnitData unitData, ColonyBase colonyBase)
    {
        assignedUnit = unitData;
        assignedBase = colonyBase;
        
        unitIcon.sprite = assignedUnit.unitIcon;
        unitNameText.text = assignedUnit.unitName;
    }

    // Bu fonksiyonu butonun OnClick event'ine bağlayacağız
    public void OnButtonClick()
    {
        if (assignedUnit != null && assignedBase != null)
        {
            // İlgili üsse, ilgili birimi üretme emrini ver
            assignedBase.StartTrainingUnit(assignedUnit);
        }
    }
}