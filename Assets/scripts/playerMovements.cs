using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class playerMovements : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Normal yürüme hızı")]
    [SerializeField] private float walkSpeed = 5f;
    [Tooltip("Shift'e basılıyken koşma hızı")]
    [SerializeField] private float runSpeed = 8f;
    [Tooltip("Karakterin ne kadar yumuşak döneceği. Düşük değer daha yavaş, yüksek değer daha hızlı döner.")]
    [SerializeField] private float turnSpeed = 15f;

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamer;
    private Transform mainCameraTransform;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isRunning;

    // YENİ: Değişkeni buraya taşıdık ki tüm fonksiyonlar erişebilsin
    private Vector3 moveDirection; 

    [Header("Zoom Settings")]
    [SerializeField] private float zoomAmount = 20f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    private float zoomInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCameraTransform = Camera.main.transform;

        playerInput = new PlayerInput();
        playerInput.PlayerController.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerInput.PlayerController.Movement.canceled += ctx => movementInput = Vector2.zero;
        playerInput.PlayerController.RUN.performed += ctx => isRunning = true;
        playerInput.PlayerController.RUN.canceled += ctx => isRunning = false;
        playerInput.CameraController.Zoom.performed += ctx => zoomInput = ctx.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        // Önce yönü hesapla, sonra bu yöne göre hareketi ve dönüşü yap
        CalculateMoveDirection();
        HandleMovement();
        HandleRotation();
    }
    
    private void LateUpdate() 
    { 
        HandleZoom(); 
    }

    private void CalculateMoveDirection()
    {
        // Kameranın ileri ve sağ yönlerini al, dikey ekseni sıfırla
        Vector3 cameraForward = mainCameraTransform.forward;
        Vector3 cameraRight = mainCameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // Kullanıcı girdisiyle bu yönleri birleştirerek nihai hareket yönünü bul
        moveDirection = (cameraForward * movementInput.y + cameraRight * movementInput.x);
    }

    private void HandleMovement()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Hareketi uygula
        Vector3 targetVelocity = moveDirection * currentSpeed;
        targetVelocity.y = rb.velocity.y; // Düşme/zıplama hızını koru
        rb.velocity = targetVelocity;
    }

    private void HandleRotation()
    {
        // Sadece hareket girdisi varsa dön
        if (moveDirection.magnitude > 0.1f)
        {
            // Bakacağı yön, hareket yönüdür
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            
            // Rigidbody'nin rotasyonunu yumuşak bir şekilde değiştir
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);
        }
    }
    
    private void HandleZoom()
    {
        if (virtualCamer == null) return;
        
        float newSize = virtualCamer.m_Lens.OrthographicSize - zoomInput * zoomAmount * Time.deltaTime;
        virtualCamer.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }

    private void OnEnable() 
    { 
        playerInput.PlayerController.Enable(); 
        playerInput.CameraController.Enable(); 
    }

    private void OnDisable() 
    { 
        playerInput.PlayerController.Disable(); 
        playerInput.CameraController.Disable(); 
    }
}