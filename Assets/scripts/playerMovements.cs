using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovements : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    
    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerInput = new PlayerInput();
        playerInput.PlayerController.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerInput.PlayerController.Movement.canceled += ctx => movementInput = Vector2.zero;
        playerInput.PlayerController.RUN.performed += ctx => isRunning = true;
        playerInput.PlayerController.RUN.canceled += ctx => isRunning = false;
    }

    private void FixedUpdate()
    {
        // Hareket yönünü kameradan değil, direkt karakterin kendi yönünden al
        Vector3 moveDirection = (transform.forward * movementInput.y + transform.right * movementInput.x);

        HandleMovement(moveDirection);
    }

    private void HandleMovement(Vector3 moveDirection)
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = moveDirection.normalized * currentSpeed;
        targetVelocity.y = rb.velocity.y; // Zıplama gibi dikey hareketleri koru
        rb.velocity = targetVelocity;
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