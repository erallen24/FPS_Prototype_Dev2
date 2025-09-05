using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] Color colorFlash;

    Color colorOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {

    }

    public void takeDamage(int damage)
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
}
