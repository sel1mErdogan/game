using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Kontrol Ayarları")]
    public Transform playerBody; // Oyuncunun gövdesini (ana objeyi) buraya sürükleyeceğiz
    public float mouseSensitivity = 100f;

    private PlayerInput playerInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerController.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerInput.PlayerController.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void Start()
    {
        // Fareyi ekranın ortasına kilitle ve görünmez yap
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Fare girdisini al
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Dikey dönüşü (aşağı/yukarı bakma) hesapla ve kameraya uygula
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Kameranın ters dönmesini engelle
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yatay dönüşü (sağa/sola bakma) direkt olarak oyuncunun gövdesine uygula
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void OnEnable()
    {
        playerInput.PlayerController.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerController.Disable();
    }
}