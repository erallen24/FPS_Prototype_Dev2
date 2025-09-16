using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] int HP;
    [SerializeField] float shield;
    [SerializeField] float shieldRegenTime;
    [SerializeField] float shieldRegenRate;
    [SerializeField] public float shootRate;
    [SerializeField] public int turnSpeed;
    [SerializeField] float FOV;
    [SerializeField] int expValue;


    [SerializeField] Color HPDamageFlash;
    [SerializeField] Color shieldDamageFlash;
    [SerializeField] public Transform shootPos;
    [SerializeField] public GameObject bullet;
    public NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform lookPos;

    public Canvas healthBar;
    public Image healthBarFill;
    public Image shieldBar;

    [SerializeField] bool isBoss = false;
    public GameObject dropItem;
    public Vector3 dropItemOffset = new Vector3(0, 0, 0);

    Color colorOrig;
    float shootTimer;
    bool playerInTrigger;
    float angleToPlayer;
    bool canSeePlayer;

    int HPOrig;
    float shieldOrig;
    float shieldTimer;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        HUDManager.instance.updateGameGoal(1);
        HPOrig = HP;
        shieldOrig = shield;
        healthBarFill.fillAmount = 1;
        healthBarFill.color = Color.green;
        StartCoroutine(DisplayHPBar(0));
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTimer < shootRate + 1f) { shootTimer += Time.deltaTime; }
        if (shieldTimer < shieldRegenTime + 1f) { shieldTimer += Time.deltaTime; }
        if (shieldTimer > shieldRegenTime && shield < shieldOrig) 
        { 
            shield += Time.deltaTime * shieldRegenRate;
            if(shield > shieldOrig) { shield = shieldOrig; }
        }

        if (playerInTrigger)
        {


            if (canSeePlayer = CanSeePlayer() && 0 != Time.timeScale)
            {
                playerDir = GameManager.instance.player.transform.position - transform.position;
                Movement(playerDir);

                Vector3 playerPos = GameManager.instance.player.transform.position;
                Vector3 lookAtPos = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z);

                shootPos.LookAt(lookAtPos);

                if (shootTimer >= shootRate) { Shoot(); }
            }
        }
        //if (isBoss && HP > 0)
        //{
        //    GameManager.instance.bossHPBar.gameObject.SetActive(true);
        //    StartCoroutine(DisplayHPBar(0));
        //}
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { playerInTrigger = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { playerInTrigger = false; }
    }

    public void TakeDamage(int damage)
    {
        shieldTimer = 0;

        if (0 < shield) { 
            shield -= damage;
        }
        else { HP -= damage; }
        StartCoroutine(flash());
        StartCoroutine(DisplayHPBar(damage));
        if (HP <= 0)
        {
            HUDManager.instance.updateGameGoal(-1);
            if (isBoss)
            {
                Instantiate(dropItem, transform.position + dropItemOffset, transform.rotation);
                HUDManager.instance.bossHPBar.gameObject.SetActive(false);
            }
            Destroy(gameObject);
            GameManager.instance.playerScript.addEXP(expValue);
        }
        FaceTarget();
        agent.SetDestination(GameManager.instance.player.transform.position);

    }

    bool CanSeePlayer()
    {

        playerDir = GameManager.instance.player.transform.position - lookPos.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        RaycastHit see;
        Debug.DrawRay(lookPos.transform.position, playerDir, Color.red);

        if (Physics.Raycast(lookPos.position, playerDir + Vector3.up, out see))
        {
            if (angleToPlayer <= FOV && see.collider.CompareTag("Player"))
            {
                //canSeePlayer = true;
                return true;
            }
        }
        //canSeePlayer = false;
        return false;
    }

    IEnumerator flash()
    {
        if (0 < shield) { model.material.color = shieldDamageFlash; }
        else { model.material.color = HPDamageFlash; }
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public virtual void Shoot()
    {
        shootTimer = 0;
        //Quaternion shootRot = Quaternion.LookRotation(new Vector3(playerDir.x, shootPos.position.y, playerDir.z));
        Instantiate(bullet, shootPos.position, shootPos.rotation);
        // SoundManager.instance.playEnemyShootSound(shootPos);
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    public virtual void Movement(Vector3 playerDir)
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (agent.remainingDistance <= agent.stoppingDistance) { FaceTarget(); }
    }


    IEnumerator DisplayHPBar(int amount)
    {
        if (!isBoss)
        {
            healthBar.gameObject.transform.rotation = Quaternion.LookRotation(playerDir);
            healthBarFill.gameObject.transform.rotation = Quaternion.LookRotation(playerDir);
            shieldBar.gameObject.transform.rotation = Quaternion.LookRotation(playerDir);

            healthBar.gameObject.SetActive(true);

            if (0 < shield) { shieldBar.fillAmount = Mathf.Lerp(((float)shield + amount) / shieldOrig, (float)shield / shieldOrig, 1f); }
            
            else {
            healthBarFill.fillAmount = Mathf.Lerp(((float)HP + amount) / HPOrig, (float)HP / HPOrig, 1f);
            // change color of health bar based on % of health left as a gradient from green to red
            healthBarFill.color = Color.Lerp(Color.red, Color.green, (float)HP / HPOrig);
            }


            yield return new WaitForSeconds(1f);
            healthBar.gameObject.SetActive(false);
        }
        else if (isBoss && MinimapManager.instance.collectedModules.Count > 0)
        {
            HUDManager.instance.bossHPBar.gameObject.SetActive(true);
            HUDManager.instance.bossHPBarFill.fillAmount = Mathf.Lerp(((float)HP + amount) / HPOrig, (float)HP / HPOrig, 1f);
            HUDManager.instance.bossHPBarFill.color = Color.Lerp(Color.red, Color.green, (float)HP / HPOrig);
            yield return new WaitForSeconds(1f);

        }
    }
}
