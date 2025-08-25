using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Kameranın takip edeceği ve etrafında döneceği hedef (Oyuncu).")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [Tooltip("Farenin hassasiyeti.")]
    [SerializeField] private float rotationSpeed = 2f;
    [Tooltip("Kameranın hedeften ne kadar uzakta duracağı.")]
    [SerializeField] private float distance = 5f;
    [Tooltip("Kameranın hedeften ne kadar yüksekte duracağı.")]
    [SerializeField] private float height = 2f;
    [Tooltip("Kameranın hedefin hareketini ne kadar yumuşak takip edeceği.")]
    [SerializeField] private float followDamping = 10f;

    private PlayerInput playerInput;
    private Vector2 mouseInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
        // Mouse'un X ve Y hareketlerini dinle
        playerInput.CameraController.Look.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        playerInput.CameraController.Look.canceled += ctx => mouseInput = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Farenin X hareketine göre yatay dönüşü hesapla
        float horizontalRotation = transform.eulerAngles.y + mouseInput.x * rotationSpeed * Time.deltaTime;

        // Hedef rotasyonu oluştur
        Quaternion targetRotation = Quaternion.Euler(0, horizontalRotation, 0);

        // Hedef pozisyonu hesapla: hedefin arkasında, belirli bir mesafe ve yükseklikte
        Vector3 targetPosition = target.position - (targetRotation * Vector3.forward * distance);
        targetPosition.y = target.position.y + height;

        // Kamera kolunun (bu objenin) pozisyonunu ve rotasyonunu yumuşakça ayarla
        transform.rotation = targetRotation;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followDamping);
    }
    
    private void OnEnable()
    {
        playerInput.CameraController.Enable();
    }

    private void OnDisable()
    {
        playerInput.CameraController.Disable();
    }
}