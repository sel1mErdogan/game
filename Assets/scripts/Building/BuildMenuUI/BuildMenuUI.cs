using UnityEngine;
using System.Collections.Generic;

public class BuildMenuUI : MonoBehaviour
{
    [Header("Menü Ayarları")]
    [SerializeField] private GameObject buildMenuPanel; // İnşaat menüsü paneli
    [SerializeField] private Transform buttonContainer; // Butonların oluşturulacağı yer
    [SerializeField] private GameObject buildingButtonPrefab; // Tek bir yapı butonu için prefab

    [Header("İnşa Edilebilir Yapılar")]
    // Buraya Inspector'dan oluşturduğun BuildingData dosyalarını sürükleyeceksin
    [SerializeField] private List<BuildingData> availableBuildings;

    void Start()
    {
        // Başlangıçta menüyü kapat ve butonları oluştur
        buildMenuPanel.SetActive(false);
        CreateBuildingButtons();
    }

    // Listemizdeki her bir yapı için bir buton oluşturur.
    private void CreateBuildingButtons()
    {
        // Önce eski butonları temizle (eğer varsa)
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Listeden yenilerini oluştur
        foreach (var building in availableBuildings)
        {
            GameObject buttonObj = Instantiate(buildingButtonPrefab, buttonContainer);
            BuildingButtonUI buttonUI = buttonObj.GetComponent<BuildingButtonUI>();
            if (buttonUI != null)
            {
                // Butona hangi yapıya ait olduğunu söyle
                buttonUI.Setup(building);
            }
        }
    }

    // Ana "İnşa Et" butonu bu fonksiyonu çağırarak menüyü açıp kapatacak.
    public void ToggleBuildMenu()
    {
        buildMenuPanel.SetActive(!buildMenuPanel.activeSelf);
    }
}