using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("HEALTH SETTINGS")]
    [Space(5)]
    [SerializeField] int HP;
    [Space(10)]

    [Header("MOVEMENT SETTINGS")]
    [Space(5)]
    [SerializeField] int movementSpeed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [Space(10)]

    [Header("SHOOT SETTINGS")]
    [Space(5)]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;

    CharacterController controller;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    float shootTimer;
    int jumpCount;
    int initialHP;
    bool isSprinting;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        UpdateMovement();
        UpdateShoot();
    }

    void Initialize()
    {
        initialHP = HP;
        controller = GetComponent<CharacterController>();
    }

    void UpdateMovement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;
        }

        moveDirection = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDirection * movementSpeed * Time.deltaTime);

        UpdateJump();

        controller.Move(playerVelocity * Time.deltaTime);

        UpdateSprint();
    }

    void UpdateJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }
    }

    void UpdateSprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            movementSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            movementSpeed /= sprintMod;
            isSprinting = false;
        }
    }

    void UpdateShoot()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage target = hit.collider.GetComponent<IDamage>();

            if (target != null)
            {
                target.TakeDamage(shootDamage);
            }
        }
    }

    public void UpdatePlayerHealthBarUI()
    {
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
}
