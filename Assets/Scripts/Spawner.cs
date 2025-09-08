using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject Spawn;
    [SerializeField] float spawnRate;
    [SerializeField] int maxObjects;

    float spawnTimer;
    int currentObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate && currentObjects < maxObjects)
        {
                SpawnObject();
        }
        if(Spawn.IsDestroyed()) { currentObjects--; }
    }

    public void SpawnObject()
    {
        spawnTimer = 0;
        currentObjects++;
        Vector3 playerDir = GameManager.instance.transform.position - transform.position;
        Instantiate(Spawn, transform.position, Quaternion.LookRotation(playerDir));
    }

    
  
}
