using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Kontrol Ayarları")]
    public Transform playerBody;
    public float mouseSensitivity = 100f;

    [Header("Kamera Sınırları")]
    [Tooltip("Kameranın aşağı ve yukarı en fazla kaç derece bakabileceğini ayarlar.")]
    [Range(10f, 20f)]
    public float verticalLookLimit = 20f;

    private float xRotation = 0f;

    void Start()
    {
        // Oyun başladığında imleç serbest ve görünür.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Fare'nin sağ tuşuna basılı tutuluyor mu diye kontrol et
        if (Mouse.current.rightButton.isPressed)
        {
            // Eğer basılı tutuluyorsa, imleci kilitle ve kamerayı döndür
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // --- EN ÖNEMLİ DEĞİŞİKLİK BURADA ---
            // Farenin o anki hareketini DOĞRUDAN OKU. Bu, "yapışmayı" engeller.
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
            float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;
            // --- BİTTİ ---

            // Dikey dönüşü (yukarı-aşağı) HESAPLA ve SINIRLA
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

            // Dikey dönüşü kameraya UYGULA
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Yatay dönüşü (sağa-sola) SINIRSIZ olarak gövdeye UYGULA
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // Eğer sağ tuşa basılmıyorsa, imleci serbest bırak
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    // Artık Awake, OnEnable ve OnDisable fonksiyonlarına ihtiyacımız yok, çünkü
    // PlayerInput sisteminin olaylarını kullanmıyoruz. Kodumuz daha basit ve daha stabil.
}