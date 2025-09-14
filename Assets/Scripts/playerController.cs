using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IPickup
{
    enum StanceState { Standing, Crouching }
    enum MovementState { Default, Sprinting }
    [SerializeField] GameObject gunModel;

    [SerializeField] CharacterController characterController;
    [SerializeField] CameraController cameraController;


    [Header("HEALTH SETTINGS")]
    [Space(10)]
    [SerializeField][UnityEngine.Range(5, 300)] private int HP;
    [Space(10)]
    [SerializeField][UnityEngine.Range(1, 10)] private int healthRegen;
    [Space(10)]

    [Header("Stamina SETTINGS")]
    [Space(10)]
    [SerializeField][UnityEngine.Range(0, 100)] private float Stamina;
    [Space(10)]

    [Header("Stamina USAGE")]
    [Space(10)]
    [SerializeField][UnityEngine.Range(0, 10)] private float staminaUsage;
    [Space(10)]

    [Header("Stamina REGEN")]
    [Space(10)]
    [SerializeField][UnityEngine.Range(0, 10)] private float staminaRegen;
    [Space(10)]



    [Header("Starting EXP")]
    [Space(10)]
    [SerializeField][UnityEngine.Range(0, 499)] private int startingEXP;
    [Space(10)]


    [Header("MOVEMENT SETTINGS")]
    [Space(10)]
    [SerializeField] private MovementState movementState;
    [Space(5)]
    [SerializeField][UnityEngine.Range(0, 25)] private int movementSpeed;
    [Space(5)]
    [SerializeField][UnityEngine.Range(0, 50)] private int sprintMovementSpeed;
    [SerializeField][UnityEngine.Range(0, 25)] private int crouchMovementSpeed;
    [Space(5)]
    [SerializeField][UnityEngine.Range(0, 10)] private int jumpSpeed;
    [SerializeField][UnityEngine.Range(0, 5)] private int jumpMax;
    [Space(5)]
    [SerializeField][UnityEngine.Range(0, 25)] private int gravity;
    [Space(10)]

    [Header("CAMERA SETTINGS")]
    [Space(10)]
    [SerializeField][UnityEngine.Range(40, 80)] private int defaultFieldOfView;
    [SerializeField][UnityEngine.Range(40, 80)] private int sprintFieldOfView;
    [Space(5)]
    [SerializeField][UnityEngine.Range(0, 10)] private float fieldOfViewSpeed;
    [Space(10)]

    [Header("STANCE SETTINGS")]
    [Space(10)]
    [SerializeField] private StanceState stanceState;
    [Space(5)]
    [SerializeField][UnityEngine.Range(1, 2)] private float standingHeight;
    [SerializeField][UnityEngine.Range(1, 2)] private float crouchingHeight;
    [Space(5)]
    [SerializeField][UnityEngine.Range(1, 10)] private float stanceSpeed;
    [Space(10)]

    [Header("GUN SETTINGS")]
    [Space(10)]
    [SerializeField][UnityEngine.Range(0, 100)] private int shootDamage;
    [SerializeField][UnityEngine.Range(0, 100)] private int shootDistance;
    [SerializeField][UnityEngine.Range(0, 1)] private float shootRate;
    [Space(5)]
    [SerializeField] LayerMask ignoreLayer;


    public int ammoCur = 5;
    [SerializeField] int ammoMax = 30; // Maximum ammo capacity
    [Header("INTERACTION SETTINGS")]
    [SerializeField][UnityEngine.Range(0, 10)] private float interactRange;

    public List<inventoryItem> inventory = new List<inventoryItem>();
    public List<WeaponData> gunList = new List<WeaponData>();


    private Vector3 movementDirection;
    private Vector3 playerVelocity;

    private int initialHP;
    private float initialStamina;
    private int maxEXP;

    private int jumpCount;
    private float shootTimer;
    private int gunListPos;
    private bool canSprint;
    private bool isReloading;
    private AudioSource audioSource;

    public bool isFullyHealed => HP >= initialHP;

    private void Start()
    {
        Initialize();
        ammoCur = ammoMax;

    }

    private void Update()
    {
        UpdateMovement();
        UpdateStance();
        UpdateShoot();
        UpdateInteract();
        UpdateCanSprint();
        UpdateStamina();
        UpdatePlayerEXPBarUI();
        UpdatePlayerStaminaBarUI();
        UpdatePlayerHealthBarUI();

        GameManager.instance.updatePlayerAmmo(ammoCur, ammoMax);

        if (Input.GetKeyDown(KeyCode.R))
        {
            AttemptReload();
        }

        if (gunList.Count > 0)
            GameManager.instance.ActivateAmmoUI();
        else
            GameManager.instance.DeactivateAmmoUI();

        SelectGun();
    }


    private void Initialize()
    {
        // setting the initial HP and stamina for bar processing //
        initialHP = HP;
        initialStamina = Stamina;
        maxEXP = 500;

        // Setting health bar to fill to the set amount at game start up
        UpdatePlayerHealthBarUI();
        UpdatePlayerEXPBarUI();

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
        SelectGun();
    }

    private void UpdateSprint()
    {
        if (Input.GetButtonDown("Sprint") && stanceState == StanceState.Standing && canSprint)
        {
            cameraController.SetFieldOfView(sprintFieldOfView, fieldOfViewSpeed);
            movementState = MovementState.Sprinting;
        }
        else if (Input.GetButtonUp("Sprint") || !canSprint)
        {
            cameraController.SetFieldOfView(defaultFieldOfView, fieldOfViewSpeed);
            movementState = MovementState.Default;
        }
    }
    public void UpdatePlayerHealthBarUI()
    {
        // updating the player health bar fill to reflect the current HP //
        GameManager.instance.playerHPBar.fillAmount = (float)HP / initialHP;
    }

    public void FillPlayerHPBar(int healAmount)
    {

        HP += healAmount * healthRegen;
        UpdatePlayerHealthBarUI();


        // Lerp the health bar fill amount to the new HP value


        if (HP > initialHP)
        {
            HP = initialHP;
            UpdatePlayerHealthBarUI();
        }

    }

    public void UpdatePlayerStaminaBarUI()
    {
        // updating the player stamina bar to show the current stamina at game start
        GameManager.instance.playerStaminaBar.fillAmount = (float)Stamina / initialStamina;
    }

    public void UpdateStamina()
    {
        if (movementState == MovementState.Sprinting && Stamina > 0)
        {
            Stamina -= staminaUsage * Time.deltaTime;
        }

        if (movementState == MovementState.Default && Stamina < initialStamina)
        {
            Stamina += staminaRegen * Time.deltaTime;
        }

    }

    private void UpdateCanSprint()
    {
        if (Stamina >= initialStamina)
        {
            canSprint = true;
        }
        else if (Stamina < 1)
        {
            canSprint = false;
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

        if (Input.GetButton("Fire1") && CanShoot() && shootTimer >= shootRate)
        {
            Shoot();
        }
        else if (ammoCur <= 0 && !isReloading)
        {
            AttemptReload();
        }
    }

    private bool CanShoot()
    {
        return ammoCur > 0 && !isReloading;
    }

    private void Shoot()
    {
        // resetting the shoot timer //
        shootTimer = 0;
        ammoCur--;

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

    private void AttemptReload()
    {
        if (isReloading || ammoCur >= ammoMax)
            return;

        StartCoroutine(ReloadSequence());
    }

    private IEnumerator ReloadSequence()
    {
        isReloading = true;

        yield return new WaitForSeconds(0.5f);
        ammoCur = ammoMax;
        isReloading = false;
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
        if (Input.GetButton("Interact"))
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

    public void GetGunStats(WeaponData gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        ChangeGun();
    }

    void ChangeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDistance = gunList[gunListPos].shootDistance;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        //audioSource.PlayOneShot(gunList[gunListPos].pickUpSound);
        UpdatePlayerHealthBarUI();


    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            ChangeGun();
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

    public void UpdatePlayerEXPBarUI()
    {
        GameManager.instance.playerEXPBar.fillAmount = (float)startingEXP / maxEXP;
    }

    public void addEXP(int amount)
    {
        startingEXP += amount;

        if (startingEXP > maxEXP)
        {
            GameManager.instance.LevelUp();
            startingEXP = startingEXP - maxEXP;
        }
    }

    public void GetPickUp(Pickup upgrade)
    {
        audioSource.PlayOneShot(upgrade.pickupSound);

        //if (upgrade.isEquippable)
        //{
        //    inventory.Add(upgrade); // Add the new pickup to the pickup list
        //}


        //else if (!upgrade.isEquippable)
        //{
        ApplyUpgradeNow(upgrade); // Apply the upgrade effects

        //}
        UpdatePlayerHealthBarUI();
    }

    void ApplyUpgradeNow(Pickup upgrade)
    {
        var type = upgrade.type;
        switch (type)
        {
            case Pickup.UpgradeType.Speed:
                movementSpeed += upgrade.speed;
                if (movementSpeed > 20)
                    movementSpeed = 20; // Cap speed to a maximum value
                break;
            case Pickup.UpgradeType.Health:
                HP += upgrade.health;
                if (HP > initialHP) HP = initialHP; // Cap health to original max health
                UpdatePlayerHealthBarUI();
                break;
            case Pickup.UpgradeType.Damage:
                gunList[gunListPos].shootDamage += upgrade.damage;
                break;
            case Pickup.UpgradeType.FireRate:
                gunList[gunListPos].shootRate = Mathf.Max(0.1f, shootRate - upgrade.shootRate); // Decrease shoot rate but not below 0.1 seconds
                break;
            case Pickup.UpgradeType.Ammo:
                gunList[gunListPos].ammoCur = Mathf.Min(gunList[gunListPos].ammoCur + upgrade.ammoAdd, gunList[gunListPos].ammoMax); // Ensure current ammo does not exceed max
                break;
            case Pickup.UpgradeType.ExtendedMag:
                gunList[gunListPos].ammoMax += upgrade.ammoMax; // Increase ammo capacity
                gunList[gunListPos].ammoCur = Mathf.Min(gunList[gunListPos].ammoCur + upgrade.ammoAdd, gunList[gunListPos].ammoMax); // Ensure current ammo does not exceed max
                break;
            default:
                Debug.LogWarning("Unknown upgrade type: " + type);
                break;
        }
    }
}
