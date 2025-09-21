using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Explosives : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] GameObject DOTitem;
    [SerializeField] ParticleSystem burnEffect;
    [SerializeField] Vector3 burnOffset;

    [SerializeField] int delay;

    private int maxHP;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHP = HP;
        
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
        
        if (HP < maxHP)
        {
            StartCoroutine(Explode());
        }
    }



    IEnumerator Explode()
    {
        burnEffect.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        Instantiate(DOTitem, new Vector3(transform.position.x, .01f, transform.position.z), Quaternion.identity);

        Destroy(gameObject);
    }
    
}
