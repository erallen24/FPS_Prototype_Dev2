using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    enum StanceState { Standing, Crouching }
    enum MovementState { Default, Sprinting }

    [Header("HEALTH SETTINGS")]
    [Space(10)]
    [SerializeField] [UnityEngine.Range(0, 100)] private int HP;
    [Space(10)]

    [Header("MOVEMENT SETTINGS")]
    [Space(10)]
    [SerializeField] private MovementState movementState;
    [Space(5)]
    [SerializeField] [UnityEngine.Range(0, 25)] private int movementSpeed;
    [Space(5)]
    [SerializeField] [UnityEngine.Range(0, 50)] private int sprintMovementSpeed;
    [SerializeField] [UnityEngine.Range(0, 25)] private int crouchMovementSpeed;
    [Space(5)]
    [SerializeField] [UnityEngine.Range(0, 10)] private int jumpSpeed;
    [SerializeField] [UnityEngine.Range(0, 5)] private int jumpMax;
    [Space(5)]
    [SerializeField] [UnityEngine.Range(0, 25)] private int gravity;
    [Space(10)]

    [Header("CAMERA SETTINGS")]
    [Space(10)]
    [SerializeField] [UnityEngine.Range(40, 80)] private int defaultFieldOfView;
    [SerializeField] [UnityEngine.Range(40, 80)] private int sprintFieldOfView;
    [Space(5)]
    [SerializeField] [UnityEngine.Range(0, 10)] private float fieldOfViewSpeed;
    [Space(10)]

    [Header("STANCE SETTINGS")]
    [Space(10)]
    [SerializeField] private StanceState stanceState;
    [Space(5)]
    [SerializeField] [UnityEngine.Range(1, 2)] private float standingHeight;
    [SerializeField] [UnityEngine.Range(1, 2)] private float crouchingHeight;
    [Space(5)]
    [SerializeField] [UnityEngine.Range(1, 10)] private float stanceSpeed;
    [Space(10)]

    [Header("SHOOT SETTINGS")]
    [Space(10)]
    [SerializeField] [UnityEngine.Range(0, 100)] private int shootDamage;
    [SerializeField] [UnityEngine.Range(0, 100)] private int shootDistance;
    [SerializeField] [UnityEngine.Range(0, 1)] private float shootRate;
    [Space(5)]
    [SerializeField] LayerMask ignoreLayer;

    [Header("INTERACTION SETTINGS")]
    [SerializeField] [UnityEngine.Range(0, 10)] private float interactRange;

    public List<inventoryItem> inventory = new List<inventoryItem>();

    private CharacterController characterController;
    private CameraController cameraController;

    private Vector3 movementDirection;
    private Vector3 playerVelocity;

    private int initialHP;
    private int jumpCount;
    private float shootTimer;


    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateMovement();
        UpdateStance();
        UpdateShoot();
        UpdateInteract();
    }

    private void Initialize()
    {
        // setting the initial HP for health bar processing //
        initialHP = HP;

        // assigning component references //
        characterController = GetComponent<CharacterController>();
        cameraController = GetComponentInChildren<CameraController>();

        // setting initial values for the camera height and FOV //
        cameraController.SetCameraHeight(standingHeight, stanceSpeed);
        cameraController.SetFieldOfView(defaultFieldOfView, fieldOfViewSpeed);
    }

    private void UpdateMovement()
    {
        // calculating and storing the movement direction //
        movementDirection = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

        // applying movement to the player //
        characterController.Move(movementDirection * GetTargetMovementSpeed() * Time.deltaTime);

        UpdateSprint();
        UpdateJump();
        UpdateGravity();
    }

    private void UpdateSprint()
    {
        if (Input.GetButtonDown("Sprint") && stanceState == StanceState.Standing)
        {
            cameraController.SetFieldOfView(sprintFieldOfView, fieldOfViewSpeed);
            movementState = MovementState.Sprinting;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            cameraController.SetFieldOfView(defaultFieldOfView, fieldOfViewSpeed);
            movementState = MovementState.Default;
        }
    }

    private void UpdateJump()
    {
        if (characterController.isGrounded)
        {
            jumpCount = 0;
        }

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }
    }

    private void UpdateGravity()
    {
        if (characterController.isGrounded)
        {
            // resetting the gravity vector if the player is grounded //
            playerVelocity = Vector3.zero;
        }
        else
        {
            // increasing gravitational force over time //
            playerVelocity.y -= gravity * Time.deltaTime;

            // applying gravitational force to the player //
            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void UpdateStance()
    {
        if (Input.GetButtonDown("Stance"))
        {
            // setting character controller values for the crouch state //
            characterController.height = crouchingHeight;
            characterController.center = new Vector3(0, crouchingHeight / 2, 0);

            // setting the camera height for the crouch state //
            cameraController.SetCameraHeight(crouchingHeight, stanceSpeed);

            // updating the stance and movement states //
            stanceState = StanceState.Crouching;
            movementState = MovementState.Default;
        }
        else if (Input.GetButtonUp("Stance"))
        {
            // setting character controller values for the stand state //
            characterController.height = standingHeight;
            characterController.center = new Vector3(0, standingHeight / 2, 0);

            // setting the camera height for the stand state //
            cameraController.SetCameraHeight(standingHeight, stanceSpeed);

            // updating the stance state
            stanceState = StanceState.Standing;
        }
    }

    private void UpdateShoot()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // resetting the shoot timer //
        shootTimer = 0;

        // performing shoot raycast //
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, shootDistance, ~ignoreLayer))
        {
            // logging the collider the raycast hit //
            Debug.Log(hit.collider.name);

            // if the collider has the IDamage interface, we store it in 'target'
            IDamage target = hit.collider.GetComponent<IDamage>();

            // null check on the target. if target is not null, we call 'TakeDamage'
            target?.TakeDamage(shootDamage);
        }
    }

    public void UpdatePlayerHealthBarUI()
    {
        // updating the player health bar fill to reflect the current HP //
        GameManager.instance.playerHPBar.fillAmount = (float)HP / initialHP;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        UpdatePlayerHealthBarUI();
        StartCoroutine(FlashDamageScreen());

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    IEnumerator FlashDamageScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    private float GetTargetMovementSpeed()
    {
        // method to return the target movement speed based on current movement/stance state //

        if (stanceState == StanceState.Crouching)
        {
            return crouchMovementSpeed;
        }
        else
        {
            if (movementState == MovementState.Sprinting)
            {
                return sprintMovementSpeed;
            }
            else
            {
                return movementSpeed;
            }
        }
    }

    public void UpdateInteract()
    {
        if(Input.GetButtonDown("Interact"))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactRange, ~ignoreLayer))
            {
                // logging the collider the raycast hit //
                Debug.Log(hit.collider.name);

                // if the collider has the IDamage interface, we store it in 'target'
                IInteractable target = hit.collider.GetComponent<IInteractable>();

                // null check on the target. if target is not null, we call 'TakeDamage'
                target?.Interact();
            }
        }
    }

    public bool HasItem(inventoryItem item)
    {
        return inventory.Contains(item);
    }

    public void AddItem(inventoryItem item)
    {
        if (!inventory.Contains(item))
        {
            inventory.Add(item);
            Debug.Log("Item added to inventory");
        }
    }
}
