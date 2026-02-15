using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private static float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float crouchMultiplier = 0.75f;

    [Header("Look Sensitivity")]
    private float mouseSensitivity => GameSettings.Instance.mouseSensitivity;
    [SerializeField] private float upDownRange = 80.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 3.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Crouch Parameters")]
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchingHeight = 1.0f;
    [SerializeField] private float crouchTransitionTime = 0.25f;

    [Header("Crouch Collision")]
    [SerializeField] private LayerMask obstructionMask;

    private bool lastCrouchState;
    private float crouchProgress = 1f;
    private float startHeight;
    private float targetHeight;
    private Vector3 startCenter;
    private Vector3 targetCenter;
    private float startCamHeight;
    private float targetCamHeight;

    [Header("Camera Parameters")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float cameraStandingHeight = 1.6f;
    [SerializeField] private float cameraCrouchingHeight = 0.8f;

    [Header("Starting Information")]
    private Vector3 gameStartPlayerPos = new Vector3(13.2f, 1.08f, 1.4f);
    private Vector3 gameStartCameraRot = new Vector3(351.826538f, 0f, 0f);

    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerInputHandler inputHandler;
    private GameObject player;
    private Vector3 currentMovement;
    private float verticalRotation;
    private static bool canControlCharacter;

    private static bool isConstantlyRunning = false;

    public static PlayerController Instance { get; private set; }

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

        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player");
        inputHandler = PlayerInputHandler.Instance;
        cameraPivot = mainCamera.transform;
        canControlCharacter = true;
        crouchProgress = 1f;
        lastCrouchState = false;
        ResetRunningConstantly();
    }

    private void Start()
    {
        // Ensure controller starts at proper standing height
        characterController.height = standingHeight;
        characterController.center = new Vector3(0, standingHeight / 2f, 0);

        // Ensure camera is at standing height
        Vector3 camPos = cameraPivot.localPosition;
        camPos.y = cameraStandingHeight;
        cameraPivot.localPosition = camPos;

        crouchProgress = 1f;
        lastCrouchState = false;
    }

    public void ForceStanding()
    {
        // Force character controller to standing height & center
        characterController.height = standingHeight;
        characterController.center = new Vector3(0, standingHeight / 2f, 0);

        // Force camera to standing height
        Vector3 camPos = cameraPivot.localPosition;
        camPos.y = cameraStandingHeight;
        cameraPivot.localPosition = camPos;

        // Reset crouch transition
        crouchProgress = 1f;
        lastCrouchState = false;
    }

    public static void EnablePlayerControl()
    {
        canControlCharacter = true;
    }

    public static void DisablePlayerControl()
    {
        canControlCharacter = false;
    }

    public static void RunningConstantly()
    {
        walkSpeed = 8.0f;
        isConstantlyRunning = true;
    }

    public static void ResetRunningConstantly()
    {
        walkSpeed = 3.0f;
        isConstantlyRunning = false;
    }

    public float GetCameraStandingHeight()
    {
        return cameraStandingHeight;
    }

    public bool CanPlayerControl() { return canControlCharacter; }

    private void Update()
    {
        try
        {
            if (canControlCharacter)
            {
                HandleMovement();
                HandleRotation();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void HandleMovement()
    {
        float multiplier = 1f;

        if (inputHandler.CrouchTriggered)
        {
            multiplier = crouchMultiplier;
        }
        else if (!isConstantlyRunning && inputHandler.SprintValue > 0)
        {
            multiplier = sprintMultiplier;
        }

        float speed = walkSpeed * multiplier;

        Vector3 inputDirections = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirections);
        worldDirection.Normalize();

        Vector3 desiredMove = worldDirection * speed;

        currentMovement.x = desiredMove.x;
        currentMovement.z = desiredMove.z;

        try
        {
            HandleJumping();
            HandleCrouching();
            characterController.Move(currentMovement * Time.deltaTime);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (inputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    void HandleCrouching()
    {
        if (characterController.isGrounded)
        {
            bool crawling = inputHandler.CrouchTriggered;

            // If player wants to stand but something is blocking, force crouch
            if (!crawling && !CanStandUp())
            {
                crawling = true;
            }

            // Only reset transition when state actually changes
            if (crawling != lastCrouchState)
            {
                lastCrouchState = crawling;
                crouchProgress = 0f;

                startHeight = characterController.height;
                targetHeight = crawling ? crouchingHeight : standingHeight;

                startCenter = characterController.center;
                targetCenter = new Vector3(0, targetHeight / 2f, 0);

                startCamHeight = cameraPivot.localPosition.y;
                targetCamHeight = crawling ? cameraCrouchingHeight : cameraStandingHeight;
            }

            // Continue transition even if player spams
            if (crouchProgress < 1f)
            {
                crouchProgress += Time.deltaTime / crouchTransitionTime;
                crouchProgress = Mathf.Clamp01(crouchProgress);

                float newHeight = Mathf.Lerp(startHeight, targetHeight, crouchProgress);
                float heightDelta = characterController.height - newHeight;
                characterController.height = newHeight;

                // Adjust center so feet stay grounded
                Vector3 newCenter = Vector3.Lerp(startCenter, targetCenter, crouchProgress);
                newCenter.y -= heightDelta / 2f;  // subtract half the height difference
                characterController.center = newCenter;

                // Camera
                Vector3 camPos = cameraPivot.localPosition;
                camPos.y = Mathf.Lerp(startCamHeight, targetCamHeight, crouchProgress);
                cameraPivot.localPosition = camPos;
            }
        }
    }

    bool CanStandUp()
    {
        float radius = characterController.radius;
        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 top = transform.position + Vector3.up * (standingHeight - radius);

        return !Physics.CheckCapsule(
            bottom,
            top,
            radius,
            obstructionMask,
            QueryTriggerInteraction.Ignore
        );
    }


    void HandleRotation()
    {
        float mouseXRotation = inputHandler.LookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= inputHandler.LookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    public void StartingPositionSet(float duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(LerpToStart(duration));
    }

    private IEnumerator LerpToStart(float duration)
    {
        Vector3 startPos = player.transform.position;
        Quaternion startRot = mainCamera.transform.localRotation;

        Vector3 targetPos = gameStartPlayerPos;
        Quaternion targetRot = Quaternion.Euler(gameStartCameraRot);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            player.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        // Ensure exact final values
        player.transform.position = targetPos;
        mainCamera.transform.localRotation = targetRot;
    }
}