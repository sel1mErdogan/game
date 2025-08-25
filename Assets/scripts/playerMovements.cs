using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovements : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float turnSpeed = 20f;

    private Transform mainCameraTransform;
    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCameraTransform = Camera.main.transform;

        playerInput = new PlayerInput();
        playerInput.PlayerController.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerInput.PlayerController.Movement.canceled += ctx => movementInput = Vector2.zero;
        playerInput.PlayerController.RUN.performed += ctx => isRunning = true;
        playerInput.PlayerController.RUN.canceled += ctx => isRunning = false;
    }

    private void FixedUpdate()
    {
        // 1. Kameranın yönünü al
        Vector3 cameraForward = mainCameraTransform.forward;
        Vector3 cameraRight = mainCameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // 2. Girdiyi kamera yönüyle birleştirerek hareket yönünü bul
        Vector3 moveDirection = (cameraForward * movementInput.y + cameraRight * movementInput.x);

        HandleMovement(moveDirection);
        HandleRotation(moveDirection);
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
        if (moveDirection.magnitude > 0.1f)
        {
            // Karakter her zaman hareket ettiği yöne baksın
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);
        }
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