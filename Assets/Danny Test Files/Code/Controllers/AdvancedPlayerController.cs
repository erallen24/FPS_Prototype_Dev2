using System.Collections;
using System.Collections.Generic;
using Player.Utilities;
using UnityEngine;

public class AdvancedPlayerController : MonoBehaviour
{
    #region PRIVATE PROPERTIES

    [SerializeField] private PlayerMovementControllerSettings movementControllerSettings;
    [Space(10)]
    [SerializeField] private PlayerCameraControllerSettings cameraControllerSettings;
    [Space(10)]
    [SerializeField] private PlayerAnimationControllerSettings animationControllerSettings;

    [Space(20)]
    [Header("Health Properties")]
    [Space(10)]
    [SerializeField][Range(5, 300)] private int HP;
    [Space(10)]
    [SerializeField][Range(1, 10)] private int healthRegen;
    [Space(20)]

    [Header("Stamina SETTINGS")]
    [Space(10)]
    [SerializeField][Range(0, 100)] private float Stamina;
    [Space(10)]

    [Header("Stamina USAGE")]
    [Space(10)]
    [SerializeField][Range(0, 10)] private float staminaUsage;
    [Space(10)]

    [Header("Stamina REGEN")]
    [Space(10)]
    [SerializeField][Range(0, 10)] private float staminaRegen;
    [Space(10)]

    [Header("Starting EXP")]
    [Space(10)]
    [SerializeField][Range(0, 499)] private int startingEXP;
    [Space(10)]

    [Header("INTERACTION SETTINGS")]
    [SerializeField][Range(0, 10)] private float interactRange;

    [Header("GUN SETTINGS")]
    [Space(10)]
    [SerializeField] GameObject gunModel;
    [SerializeField][UnityEngine.Range(0, 100)] private int shootDamage;
    [SerializeField][UnityEngine.Range(0, 100)] private int shootDistance;
    [SerializeField][UnityEngine.Range(0, 1)] private float shootRate;
    [Space(5)]
    [SerializeField] LayerMask ignoreLayer;
    public int ammoCur = 5;
    [SerializeField] int ammoMax = 30; // Maximum ammo capacity

    public List<inventoryItem> inventory = new List<inventoryItem>();
    public List<WeaponData> gunList = new List<WeaponData>();


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
    public bool isLowHealth => HP <= initialHP * 0.3f;

    #endregion

    #region PROPERTY GETTERS

    public float DefaultMovementSpeed => movementControllerSettings.data.defaultMovementSpeed;
    public float SprintMovementSpeed => movementControllerSettings.data.sprintMovementSpeed;
    public float GravityForce => movementControllerSettings.data.gravityForce;
    public int JumpForce => movementControllerSettings.data.jumpForce;
    public int MaxJumpCount => movementControllerSettings.data.maxJumpCount;
    public float GroundCheckRadius => movementControllerSettings.data.groundCheckRadius;
    public Vector3 GroundCheckRadiusOffset => movementControllerSettings.data.groundCheckRadiusOffset;
    public LayerMask GroundLayer => movementControllerSettings.data.groundLayer;

    public PlayerGroundedState GroundedState { get { return movementControllerSettings.groundedState; } set { movementControllerSettings.groundedState = value; } }
    public PlayerLocomotionState LocomotionState { get { return movementControllerSettings.locomotionState; } set { movementControllerSettings.locomotionState = value; } }

    public Camera PlayerCamera => cameraControllerSettings.camera;
    public Transform CameraRig => cameraControllerSettings.cameraRigTransform;
    public Vector2 CameraRotationClamp => cameraControllerSettings.data.cameraRotationClamp;
    public Vector2 CameraSensitivity => cameraControllerSettings.data.cameraSensitivity * 100;

    public Animator Animator => animationControllerSettings.animator;
    public Transform AnimatorLookAt => animationControllerSettings.animatorLookAtTransform;
    public Transform RightHandIK => animationControllerSettings.rightHandIKTransform;
    public Transform LeftHandIK => animationControllerSettings.leftHandIKTransform;
    public Transform LeftHandIKTarget => animationControllerSettings.leftHandIKTargetTransform;

    #endregion

    #region CONTROLLERS

    public PlayerInputController InputController { get; private set; }
    public PlayerMovementController MovementController { get; private set; }
    public PlayerCameraController CameraController { get; private set; }
    public PlayerAnimationController AnimationController { get; private set; }

    #endregion

    #region METHODS
    private void InitializeControllers()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputController = new PlayerInputController(this);
        MovementController = new PlayerMovementController(this, movementControllerSettings);
        CameraController = new PlayerCameraController(this, cameraControllerSettings);
        AnimationController = new PlayerAnimationController(this, animationControllerSettings);
    }
    private void UpdateControllers()
    {
        InputController?.Update();
        MovementController?.Update();
        CameraController?.Update();
        AnimationController?.Update();
    }
    private void LateUpdateControllers()
    {
        CameraController?.LateUpdate();
        AnimationController?.LateUpdate();
    }


    public void UpdatePlayerHealthBarUI()
    {
        // updating the player health bar fill to reflect the current HP //
        HUDManager.instance.playerHPBar.fillAmount = (float)HP / initialHP;
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
        HUDManager.instance.playerStaminaBar.fillAmount = (float)Stamina / initialStamina;
    }
    public void UpdateStamina()
    {
        if (LocomotionState == PlayerLocomotionState.Sprinting && Stamina > 0)
        {
            Stamina -= staminaUsage * Time.deltaTime;
        }

        if (LocomotionState == PlayerLocomotionState.Default && Stamina < initialStamina)
        {
            Stamina += staminaRegen * Time.deltaTime;
        }

    }


    public void UpdatePlayerEXPBarUI()
    {
        HUDManager.instance.playerEXPBar.fillAmount = (float)startingEXP / maxEXP;
    }
    public void addEXP(int amount)
    {
        startingEXP += amount;

        if (startingEXP > maxEXP)
        {
            HUDManager.instance.LevelUp();
            startingEXP = startingEXP - maxEXP;
            maxEXP = maxEXP * 2;
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
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, shootDistance, ~ignoreLayer))
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
    public void GetGunStats(WeaponData gunStat, inventoryItem gun)
    {
        if (HasItem(gun))
            return; // Player already has this gun, do not pick up again
        gunList.Add(gunStat);
        gunListPos = gunList.Count - 1;
        AddItem(gun);

        ChangeGun();
    }
    void ChangeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDistance = gunList[gunListPos].shootDistance;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        SoundManager.instance.soundSource.PlayOneShot(gunList[gunListPos].pickUpSound);
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


    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (isLowHealth && !InfoManager.instance.IsInfoShowing())
        {

            InfoManager.instance.ShowMessage("WARNING!", "Health Critical!", Color.red, 2);
        }

        UpdatePlayerHealthBarUI();
        StartCoroutine(FlashDamageScreen());

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }
    IEnumerator FlashDamageScreen()
    {
        HUDManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        HUDManager.instance.playerDamageScreen.SetActive(false);
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


    public void ApplyUpgradeNow(PickupData pickup, inventoryItem.ItemType itemType)
    {

        switch (itemType)
        {
            case inventoryItem.ItemType.AdrenalineShot:
                /*
                movementSpeed += pickup.speed;
                if (movementSpeed > 20)
                    movementSpeed = 20; // Cap speed to a maximum value
                */
                break;
            case inventoryItem.ItemType.Shield:
                HP += pickup.health;
                if (HP > initialHP)
                    HP = initialHP; // Cap health to initial value
                UpdatePlayerHealthBarUI();
                break;
            case inventoryItem.ItemType.EMP:
                // Implement EMP effect here
                break;
            case inventoryItem.ItemType.CloakingDevice:
                // Implement Cloaking effect here
                break;
            case inventoryItem.ItemType.StunGrenade:
                // Implement Stun Grenade effect here

                break;
            case inventoryItem.ItemType.Bomb:
            // Implement Bomb effect here

            case inventoryItem.ItemType.Armor:
                break;
            case inventoryItem.ItemType.Accessory:
                break;
            case inventoryItem.ItemType.Misc:
                break;


        }
    }

    #endregion

    #region MONOBEHAVIOUR

    private void Start()
    {
        InitializeControllers();

        // setting the initial HP and stamina for bar processing //
        initialHP = HP;
        initialStamina = Stamina;
        maxEXP = 500;

        // Setting health bar to fill to the set amount at game start up
        UpdatePlayerHealthBarUI();
        UpdatePlayerEXPBarUI();

        ammoCur = ammoMax;
        InfoManager.instance.ShowMessage("ESCAPE!", "Use WASD to move, Shift to sprint, Space to jump, Ctrl to crouch, Left Click to shoot, R to reload, E to interact, Mouse Wheel to switch weapons.", Color.lightBlue, 10);
    }

    private void Update()
    {
        UpdateControllers();

        UpdateShoot();
        UpdateInteract();
        UpdateStamina();
        UpdatePlayerEXPBarUI();
        UpdatePlayerStaminaBarUI();
        UpdatePlayerHealthBarUI();

        HUDManager.instance.updatePlayerAmmo(ammoCur, ammoMax);
        HUDManager.instance.updatePlayerEXP(startingEXP, maxEXP);
        HUDManager.instance.updateHealthValue(HP);
        HUDManager.instance.updateStaminaValue((int)Stamina);

        if (Input.GetKeyDown(KeyCode.R))
        {
            AttemptReload();
        }

        if (gunList.Count > 0)
            HUDManager.instance.ActivateAmmoUI();
        else
            HUDManager.instance.DeactivateAmmoUI();

        SelectGun();
    }

    private void LateUpdate()
    {
        LateUpdateControllers();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        AnimationController?.OnAnimatorIK();
    }

    private void OnDrawGizmos()
    {
        MovementController?.DrawGizmos();
    }

    #endregion
}
