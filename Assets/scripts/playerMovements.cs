using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovements : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerInput playerInput;
    [SerializeField] private float walkSpeed, runSpeed,turnSpeed;
    Vector2 movementInput;
    Vector3 currentMovement, toIso;
    bool ismovementPressed, isRunPressed;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerController.Movement.started += OnMove;
        playerInput.PlayerController.Movement.performed += OnMove;
        playerInput.PlayerController.Movement.canceled += OnMove;

        playerInput.PlayerController.RUN.started += OnRun;
        playerInput.PlayerController.RUN.canceled += OnRun;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        currentMovement.x = movementInput.x;
        currentMovement.z = movementInput.y;
        ismovementPressed = movementInput.x != 0 || movementInput.y != 0;
    }
    private void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }
    private void FixedUpdate()
    {
        PlayerMove();
    }
    private void Update()
    {
        RotationProcess();
    }
    private void PlayerMove()
    {
        if (!isRunPressed)
        {
            rb.MovePosition(transform.position + toIso * currentMovement.normalized.magnitude * walkSpeed * Time.deltaTime);
        }
        else
        {
            rb.MovePosition(transform.position + currentMovement.normalized * runSpeed * Time.deltaTime);

        }
    }
    private void RotationProcess()
    {
        if (!ismovementPressed) return;
        Quaternion isoAnlge = Quaternion.Euler(0,45,0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(isoAnlge);
        toIso = isoMatrix.MultiplyPoint3x4(currentMovement);
        
        Quaternion targerRotation = Quaternion.LookRotation(toIso, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targerRotation, turnSpeed * Time.deltaTime);
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
