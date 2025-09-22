using UnityEngine;

[System.Serializable]
public class SpawnGroup
{
    [Header("Spawn Group Settings")]
    public string groupName { get; set; }
    public GameObject[] objects;
    public int count;
    public int spawnRate; // % chance to spawn each interval or time?
    public Transform[] spawnPos;
}
public class Spawner : MonoBehaviour
{
    [SerializeField] private bool autoSpawn = false;
    [SerializeField] private GameObject[] autoSpawnObjects;
    [SerializeField] private int autoSpawnCount = 5;
    [SerializeField] private int spawnRate = 2; // per second
    [SerializeField] private float autoSpawnRadius = 5;

    [SerializeField] private SpawnGroup mainGroup;
    [SerializeField] private SpawnGroup secondaryGroup;
    [SerializeField] private SpawnGroup tertiaryGroup;
    [SerializeField] private SpawnGroup quaternaryGroup;
    [SerializeField] private GameObject[] bossObjects;
    [SerializeField] private Transform[] bossSpawnPoints;

    // Boss Settings (Should we leave this out of the editor code?)
    [SerializeField] private bool isBossSpawner = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //HUDManager.instance.updateGameGoal(numToSpawn);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnNow()
    {
        if (autoSpawn)

        {
            for (int i = 0; i < autoSpawnCount; i++)
            {
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * autoSpawnRadius;
                spawnPos.y = transform.position.y;
                Instantiate(autoSpawnObjects[Random.Range(0, autoSpawnObjects.Length)], spawnPos, Quaternion.identity);
            }
        }
        else
        {
            SpawnGroupObjects(mainGroup);


            SpawnGroupObjects(secondaryGroup);


            SpawnGroupObjects(tertiaryGroup);


            SpawnGroupObjects(quaternaryGroup);
        }

        if (isBossSpawner && bossObjects.Length > 0 && bossSpawnPoints.Length > 0)
        {
            foreach (Transform spawnPos in bossSpawnPoints)
            {
                Instantiate(bossObjects[Random.Range(0, bossObjects.Length)], spawnPos.position, spawnPos.rotation);
            }
        }

    }

    //Cursor.lockState = CursorLockMode.Locked;
    private void SpawnGroupObjects(SpawnGroup group)
    {
        if (group == null || group.objects.Length == 0 || group.spawnPos.Length == 0 || group.count <= 0)
            return;

        for (int i = 0; i < group.count; i++)
        {
            if (Random.Range(0, 100) <= group.spawnRate)
            {
                Transform spawnPos = group.spawnPos[Random.Range(0, group.spawnPos.Length)];
                GameObject objToSpawn = group.objects[Random.Range(0, group.objects.Length)];
                Instantiate(objToSpawn, spawnPos.position, spawnPos.rotation);
            }
        }
    }
}
