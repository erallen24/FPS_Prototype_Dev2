using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] int HP;
    [SerializeField] public float shootRate;
    [SerializeField] public int turnSpeed;
    [SerializeField] float FOV;
    [SerializeField] int expValue;


    [SerializeField] Color colorFlash;
    [SerializeField] public Transform shootPos;
    [SerializeField] public GameObject bullet;
    public NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform lookPos;

    [SerializeField] Canvas healthBar;
    [SerializeField] Image healthBarFill;

    [SerializeField] bool isBoss = false;
    public GameObject dropItem;
    public Vector3 dropItemOffset = new Vector3(0, 0, 0);

    Color colorOrig;
    float shootTimer;
    bool playerInTrigger;
    float angleToPlayer;
    bool canSeePlayer;

    int HPOrig;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
        HPOrig = HP;
        StartCoroutine(DisplayHPBar(0));
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTimer < shootRate + 1) { shootTimer += Time.deltaTime; }
        if (playerInTrigger)
        {


            if (CanSeePlayer() && 0 != Time.timeScale)
            {
                playerDir = GameManager.instance.player.transform.position - transform.position;
                Movement(playerDir);

                Vector3 playerPos = GameManager.instance.player.transform.position;
                Vector3 lookAtPos = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z);

                shootPos.LookAt(lookAtPos);

                if (shootTimer >= shootRate) { Shoot(); }
            }
        }
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
        if (HP > 0)
        {
            HP -= damage;
            StartCoroutine(flash());
            StartCoroutine(DisplayHPBar(damage));
        }
        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            if (isBoss)
            {
                Instantiate(dropItem, transform.position + dropItemOffset, transform.rotation);
            }
            Destroy(gameObject);
            GameManager.instance.playerScript.addEXP(expValue);
        }

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
                return true;
            }
        }
        return false;
    }

    IEnumerator flash()
    {
        model.material.color = colorFlash;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public virtual void Shoot()
    {
        shootTimer = 0;
        //Quaternion shootRot = Quaternion.LookRotation(new Vector3(playerDir.x, shootPos.position.y, playerDir.z));
        Instantiate(bullet, shootPos.position, shootPos.rotation);
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
        healthBar.gameObject.SetActive(true);
        healthBarFill.fillAmount = Mathf.Lerp(((float)HP + amount) / HPOrig, (float)HP / HPOrig, 1f);
        // change color of health bar based on % of health left as a gradient from green to red
        healthBarFill.color = Color.Lerp(Color.red, Color.green, (float)HP / HPOrig);


        yield return new WaitForSeconds(1f);
        healthBar.gameObject.SetActive(false);
    }
}
