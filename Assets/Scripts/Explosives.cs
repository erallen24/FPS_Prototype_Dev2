using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Explosives : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] GameObject DOTitem;
    [SerializeField] int damageAmount;
    [SerializeField] ParticleSystem burnEffect;

    private Renderer objRenderer;

    private int maxHP;

    private bool isBurning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHP = HP;
        objRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isBurning && HP < maxHP)
        {
            burnEffect.gameObject.SetActive(true);
            StartCoroutine(Burn(damageAmount));
        }
        if ( HP <= 0)
        {
            Instantiate(DOTitem, new Vector3(transform.position.x, .01f, transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;

    }

  

    IEnumerator Burn(int damage)
    {
        isBurning = true;

        while (HP > 0)
        {
            HP -= damage;

            yield return new WaitForSeconds(1f);
        }

        isBurning = false;
    }
    
}
