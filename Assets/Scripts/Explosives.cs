using UnityEngine;

public class Explosives : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] GameObject DOTitem;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
}
