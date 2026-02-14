using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour
{

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Axction Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string escape = "Escape";
    [SerializeField] private string shoot = "Shoot";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction escapeAction;
    private InputAction shootAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }
    public float CrouchValue { get; private set; }
    public float SprintValue { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool EscapeTriggered { get; private set; }
    public bool ShootTriggered { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        crouchAction = playerControls.FindActionMap(actionMapName).FindAction(crouch);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        interactAction = playerControls.FindActionMap(actionMapName).FindAction(interact);
        escapeAction = playerControls.FindActionMap(actionMapName).FindAction(escape);
        shootAction = playerControls.FindActionMap(actionMapName).FindAction(shoot);
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        moveAction.performed += contect => MoveInput = contect.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += contect => LookInput = contect.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        crouchAction.performed += context => CrouchTriggered = true;
        crouchAction.performed += context => CrouchValue = context.ReadValue<float>();
        crouchAction.canceled += context => CrouchTriggered = false;
        crouchAction.canceled += context => CrouchValue = 0f;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0f;

        interactAction.performed += context => InteractTriggered = true;
        interactAction.canceled += context => InteractTriggered = false;

        escapeAction.performed += context => EscapeTriggered = true;
        escapeAction.canceled += context => EscapeTriggered = false;

        shootAction.performed += context => ShootTriggered = true;
        shootAction.canceled += context => ShootTriggered = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();
        sprintAction.Enable();
        interactAction.Enable();
        escapeAction.Enable();
        shootAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
        sprintAction.Disable();
        interactAction.Disable();
        escapeAction.Disable();
        shootAction.Disable();
    }
}