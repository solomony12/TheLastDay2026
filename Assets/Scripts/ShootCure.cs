using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootCure : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject player;
    private GameObject gun;

    [Header("Scanner Info")]
    private Vector3 gunOrigPos = new Vector3(0.47f, -0.362f, 0.748f);
    private Vector3 gunOrigRot = Vector3.zero;

    private PlayerInputHandler inputHandler;
    private HoverCaptions hoverCaptions;
    private CureSystem cureSystem;

    [Header("Distance to Shoot")]
    private float maxPlayerObjectDistance = 5f;
    private float maxCameraObjectDistance = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag(Constants.playerTag);
        gun = GameObject.FindWithTag(Constants.gunTag);
        gun.transform.position = gunOrigPos;
        gun.transform.rotation = Quaternion.Euler(gunOrigRot);
        hoverCaptions = HoverCaptions.Instance;
        inputHandler = PlayerInputHandler.Instance;
        cureSystem = CureSystem.Instance;

        if (inputHandler == null)
            Debug.LogError("PlayerInputHandler.Instance is NULL!");

        if (hoverCaptions == null)
            Debug.LogError("HoverCaptions.Instance is NULL!");

        shootClip = Resources.Load<AudioClip>("Sounds/correct-156911");
    }

    void Update()
    {
        HandleShoot();
    }

    void HandleShoot()
    {
        if (SettingsMenuUI.SettingsIsOpen)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        bool isNearZombie = false;

        // Close enough to player
        if (Physics.Raycast(ray, out hit, maxPlayerObjectDistance))
        {
            // Zombie
            if (hit.collider.CompareTag(Constants.zombieTag))
            {
                float dist = Vector3.Distance(player.transform.position, hit.collider.transform.position);

                // Close enough for camera to see
                if (dist <= maxCameraObjectDistance)
                {
                    string zombieName = hit.collider.name;
                    //Debug.Log("Near zombie: " + zombieName);

                    isNearZombie = true;
                    hoverCaptions.ShowCaptions("IN RANGE OF TARGET");

                    // Shoot zombie
                    if (inputHandler.ShootTriggered)
                    {
                        if (cureSystem.GetAmountOfCuresLeft() > 0)
                        {
                            cureSystem.DecrementCure();
                            cureSystem.ZombieCured(zombieName); // cure system
                            hit.collider.gameObject.GetComponent<ZombieAI>().ZombieCured(); // zombie ai

                        }
                        else
                        {
                            Captions.Instance.TimedShowCaptions("You're out of cures.", 3f);
                        }
                    }
                }
            }
        }

        if (!isNearZombie)
        {
            hoverCaptions.HideCaptions();
            return;
        }
    }
}