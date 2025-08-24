using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class playerMovements : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float turnSpeed = 20f; // Dönüşleri biraz daha yumuşattık

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamer;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isRunning;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomAmount = 20f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    private float zoomInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerInput = new PlayerInput();
        playerInput.PlayerController.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerInput.PlayerController.Movement.canceled += ctx => movementInput = Vector2.zero;
        playerInput.PlayerController.RUN.performed += ctx => isRunning = true;
        playerInput.PlayerController.RUN.canceled += ctx => isRunning = false;
        playerInput.CameraController.Zoom.performed += ctx => zoomInput = ctx.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        // 1. Kullanıcı girdisinden temel hareket yönünü al (yukarı/aşağı, sağ/sol)
        Vector3 moveInput = new Vector3(movementInput.x, 0, movementInput.y);

        // 2. Bu yönü, izometrik kameranın 45 derecelik açısına göre DÖNDÜR
        Vector3 moveDirection = Quaternion.Euler(0, 45, 0) * moveInput;

        // 3. Hareketi uygula
        HandleMovement(moveDirection);
        
        // 4. Dönüşü uygula
        HandleRotation(moveDirection);
    }
    
    private void LateUpdate() 
    { 
        HandleZoom(); 
    }

    private void HandleMovement(Vector3 moveDirection)
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = moveDirection.normalized * currentSpeed;
        targetVelocity.y = rb.velocity.y;
        rb.velocity = targetVelocity;
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        // Sadece hareket girdisi varsa dön
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);
        }
    }
    
    private void HandleZoom()
    {
        if (virtualCamer == null) return;
        float newSize = virtualCamer.m_Lens.OrthographicSize - zoomInput * zoomAmount * Time.deltaTime;
        virtualCamer.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }

    private void OnEnable() { playerInput.PlayerController.Enable(); playerInput.CameraController.Enable(); }
    private void OnDisable() { playerInput.PlayerController.Disable(); playerInput.CameraController.Disable(); }
}