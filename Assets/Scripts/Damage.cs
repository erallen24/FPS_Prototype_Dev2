using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{

    enum damageType { moving, stationary, DOT, homing }

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int lifespan;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (damageType.moving == type || damageType.homing == type)
        {
            Destroy(gameObject, lifespan);

            if (damageType.moving == type)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (damageType.homing == type)
        {
            rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) { return; }

        IDamage dmg = other.GetComponent<IDamage>();

        if (null == dmg || damageType.DOT == type) { return; }

        dmg.TakeDamage(damageAmount);

        if (damageType.stationary != type)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) { return; }

        IDamage dmg = other.GetComponent<IDamage>();

        if (null == dmg || damageType.DOT != type) { return; }
        if (!isDamaging) { StartCoroutine(DamageOther(dmg)); }
    }

    IEnumerator DamageOther(IDamage dmg)
    {
        isDamaging = true;
        dmg.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

}
