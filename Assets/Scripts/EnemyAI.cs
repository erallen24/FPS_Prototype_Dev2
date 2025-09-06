using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] int HP;
    [SerializeField] float shootRate;
    [SerializeField] int turnSpeed;

   
    
    [SerializeField] Color colorFlash;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    Color colorOrig;
    float shootTimer;
    bool playerInTrigger;

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
        shootTimer += Time.deltaTime;
        playerDir = GameManager.instance.player.transform.position - transform.position;

        if (playerInTrigger)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance) { FaceTarget(); }
            if (shootTimer >= shootRate) { Shoot(); }
        }
    }

    void OnDestroy()
    {
        GameManager.instance.updateGameGoal(-1);
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

    IEnumerator flash()
    {
        model.material.color = colorFlash;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    void Shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }
}
