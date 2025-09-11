using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] int HP;
    [SerializeField] public float shootRate;
    [SerializeField] public int turnSpeed;
    [SerializeField] float FOV;
   
    
    [SerializeField] Color colorFlash;
    [SerializeField] public Transform shootPos;
    [SerializeField] public GameObject bullet;
    public NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform lookPos;

    [SerializeField] bool isBoss = false;
    public GameObject dropItem;
    public Vector3 dropItemOffset = Vector3(0,0,0);

    Color colorOrig;
    float shootTimer;
    bool playerInTrigger;
    float angleToPlayer;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if ( shootTimer < shootRate + 1) { shootTimer += Time.deltaTime; }
        if (playerInTrigger)
        {
            

            if (CanSeePlayer() && 0 != Time.timeScale)
            {
                Movement();
                if (shootTimer >= shootRate) { Shoot(); }
            }
        }
    }

    void OnDestroy()
    {
        GameManager.instance.updateGameGoal(-1);
        if (isBoss)
        {
            Instantiate(dropItem,transform.position + dropItemOffset, transform.rotation);
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
        if (HP > 0) {
            HP -= damage;
            StartCoroutine(flash());
        }
        if (HP <= 0 ) { Destroy(gameObject); }
        
    }

    bool CanSeePlayer()
    {
        RaycastHit see;
        playerDir = GameManager.instance.player.transform.position - lookPos.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        if(Physics.Raycast(lookPos.position, playerDir, out see))
        {
            if(angleToPlayer <= FOV && see.collider.CompareTag("Player"))
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
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    public virtual void Movement()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (agent.remainingDistance <= agent.stoppingDistance) { FaceTarget(); }
    }
}
