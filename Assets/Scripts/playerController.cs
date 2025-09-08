using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    enum StanceState { Standing, Crouching }
    enum MovementState { Default, Sprinting }

    [Header("HEALTH SETTINGS")]
    [Space(10)]
    [SerializeField] [Range(0, 100)] private int HP;
    [Space(10)]

    [Header("MOVEMENT SETTINGS")]
    [Space(10)]
    [SerializeField] private MovementState movementState;
    [Space(5)]
    [SerializeField] [Range(0, 25)] private int movementSpeed;
    [Space(5)]
    [SerializeField] [Range(0, 50)] private int sprintMovementSpeed;
    [SerializeField] [Range(0, 25)] private int crouchMovementSpeed;
    [Space(5)]
    [SerializeField] [Range(0, 10)] private int jumpSpeed;
    [SerializeField] [Range(0, 5)] private int jumpMax;
    [Space(5)]
    [SerializeField] [Range(0, 25)] private int gravity;
    [Space(10)]

    [Header("CAMERA SETTINGS")]
    [Space(10)]
    [SerializeField] [Range(40, 80)] private int defaultFieldOfView;
    [SerializeField] [Range(40, 80)] private int sprintFieldOfView;
    [Space(5)]
    [SerializeField] [Range(0, 10)] private float fieldOfViewSpeed;
    [Space(10)]

    [Header("STANCE SETTINGS")]
    [Space(10)]
    [SerializeField] private StanceState stanceState;
    [Space(5)]
    [SerializeField] private float standingHeight;
    [SerializeField] private float crouchingHeight;
    [Space(5)]
    [SerializeField] private float stanceSpeed;
    [Space(10)]

    [Header("SHOOT SETTINGS")]
    [Space(10)]
    [SerializeField] [Range(0, 100)] private int shootDamage;
    [SerializeField] [Range(0, 100)] private int shootDistance;
    [SerializeField] [Range(0, 1)] private float shootRate;
    [Space(5)]
    [SerializeField] LayerMask ignoreLayer;


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
    }

    private void Initialize()
    {
        initialHP = HP;

        characterController = GetComponent<CharacterController>();
        cameraController = GetComponentInChildren<CameraController>();

        cameraController.SetCameraHeight(standingHeight, stanceSpeed);
        cameraController.SetFieldOfView(defaultFieldOfView, fieldOfViewSpeed);
    }

    private void UpdateMovement()
    {
        movementDirection = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

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
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void UpdateStance()
    {
        if (Input.GetButtonDown("Stance"))
        {
            characterController.height = crouchingHeight;
            characterController.center = new Vector3(0, crouchingHeight / 2, 0);

            cameraController.SetCameraHeight(crouchingHeight, stanceSpeed);

            stanceState = StanceState.Crouching;
            movementState = MovementState.Default;
        }
        else if (Input.GetButtonUp("Stance"))
        {
            characterController.height = standingHeight;
            characterController.center = new Vector3(0, standingHeight / 2, 0);

            cameraController.SetCameraHeight(standingHeight, stanceSpeed);

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
}
