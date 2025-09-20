using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Explosives : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] GameObject DOTitem;
    [SerializeField] int damageAmount;
    [SerializeField] ParticleSystem burnEffect;
    [SerializeField] int explosionForce;
    [SerializeField] int explosionRadius;
    [SerializeField] int delay;

    private Renderer objRenderer;

    private int maxHP;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHP = HP;
        objRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

     
        if ( HP <= 0)
        {
            
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        
        if(HP <= 0)
        {
            StartCoroutine(Explode());
        }
    }



    IEnumerator Explode()
    {
        burnEffect.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
                if (DOTitem != null)
                {
                    Instantiate(DOTitem, new Vector3(transform.position.x, .01f, transform.position.z), Quaternion.identity);
                }
                yield return new WaitForSeconds(2);
                rb.isKinematic = true;
            }
        }
        Destroy(gameObject);

    }
    
}
