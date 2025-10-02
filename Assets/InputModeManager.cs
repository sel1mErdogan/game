using UnityEngine;

public class InputModeManager : MonoBehaviour
{
    // Bu script'e her yerden kolayca erişebilmek için bir "singleton" yapıyoruz
    public static InputModeManager Instance;

    [Header("Kontrol Edilecek Scriptler")]
    public MouseLook mouseLookScript;
    public playerMovements playerMovementsScript;

    void Awake()
    {
        // Singleton kurulumu
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Oyun her zaman "Oyun Modu" ile başlasın
        EnterGameplayMode();
    }

    // Menü açıldığında bu fonksiyon çağrılacak
    public void EnterUIMode()
    {
        // Oyuncu kontrol scriptlerini devre dışı bırak
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        if (playerMovementsScript != null) playerMovementsScript.enabled = false;

        // Fareyi serbest bırak ve görünür yap
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Menü kapandığında bu fonksiyon çağrılacak
    public void EnterGameplayMode()
    {
        // Oyuncu kontrol scriptlerini tekrar aktif et
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        if (playerMovementsScript != null) playerMovementsScript.enabled = true;

        // Fareyi kilitle ve görünmez yap
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}