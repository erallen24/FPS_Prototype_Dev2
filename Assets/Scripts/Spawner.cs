using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnGroup
{
    [Tooltip("Label for this group (used for debugging or UI)")]
    public string groupName;

    [Tooltip("Prefabs to spawn from this group")]
    public GameObject[] objects;

    [Tooltip("Number of objects to attempt spawning")]
    public int count = 1;

    [Tooltip("Chance (0–100%) to spawn each object per attempt")]
    [Range(0, 60)]
    public int spawnRate = 5;

    [Tooltip("Number of objects to spawn at a time")]
    public int spawnsPerInterval = 1;

    [Tooltip("Spawn positions for this group")]
    public Transform[] spawnPositions;
    [HideInInspector] public int spawnedCount = 0;
}

public class Spawner : MonoBehaviour
{
    [Header("Spawner Mode")]
    [Tooltip("Enable automatic spawning at runtime")]
    [SerializeField] private bool autoSpawn = false;
    [SerializeField] private GameObject[] autoSpawnObjects;
    [SerializeField] private int autoSpawnCount = 5;
    [SerializeField] private int autoSpawnObjAtATime = 5;
    [SerializeField] private int autoSpawnRate = 2;
    [SerializeField] private float autoSpawnRadius = 5f;

    //[Header("Manual Spawn Groups")]

    [SerializeField] private SpawnGroup mainGroup;
    [SerializeField] private SpawnGroup secondaryGroup;
    [SerializeField] private SpawnGroup tertiaryGroup;
    [SerializeField] private SpawnGroup quaternaryGroup;


    //[Header("Boss Settings")]
    [SerializeField] private bool isBossSpawner = false;
    [SerializeField] private GameObject[] bossObjects;
    [SerializeField] private Transform[] bossSpawnPositions;
    [SerializeField][Range(0, 120f)] private float bossSpawnRate = 10f;
    [SerializeField] private int bossesAtATime = 1;
    [Tooltip("Total spawns completed is normalized from (0–100%)")]
    [SerializeField][Range(0, 1f)] private float spawnAtCompletionProgess = 0.8f;



    private Dictionary<SpawnGroup, float> groupTimers = new();

    private float autoSpawnTimer = 0;
    private int autoSpawnedCount = 0;


    private float bossTimer = 0;
    private int bossSpawnCount = 0;


    private bool bossSpawned = false;
    private bool startSpawning = false;
    private void Start()
    {
        groupTimers[mainGroup] = 0;
        groupTimers[secondaryGroup] = 0;
        groupTimers[tertiaryGroup] = 0;
        groupTimers[quaternaryGroup] = 0;

        if (autoSpawn)
        {
            autoSpawnTimer = 0;
        }

        if (isBossSpawner)
        {
            bossSpawned = false;
            bossSpawnCount = 0;
        }
    }

    private void Update()
    {
        if (startSpawning)
            SpawnNow();
    }

    private void TrySpawnGroup(SpawnGroup group, float delta)
    {
        if (group == null || group.objects == null || group.spawnPositions == null ||
            group.objects.Length == 0 || group.spawnPositions.Length == 0 || group.count <= 0)
            return;

        groupTimers[group] += delta;
        if (groupTimers[group] >= group.spawnRate) // Check every second
        {
            groupTimers[group] = 0f;
            SpawnGroupObjects(group);
        }
    }

    private void TryAutoSpawn(float delta)
    {
        autoSpawnTimer += delta;
        if (autoSpawnTimer >= autoSpawnRate)
        {
            autoSpawnTimer = 0f;

            SpawnAutoObjects();
        }
    }

    private void SpawnAutoObjects()
    {
        if (autoSpawnObjects == null || autoSpawnObjects.Length == 0 || autoSpawnCount <= 0)
        {
            Debug.LogWarning("Auto spawn settings are not properly configured.");
            return;
        }
        int remaining = autoSpawnCount - autoSpawnedCount;
        int toSpawn = Mathf.Min(remaining, autoSpawnObjAtATime);
        for (int i = 0; i < toSpawn; i++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * autoSpawnRadius;
            spawnPos.y = transform.position.y;
            GameObject prefab = autoSpawnObjects[Random.Range(0, autoSpawnObjects.Length)];
            Instantiate(prefab, spawnPos, Quaternion.identity);
            autoSpawnedCount++;
        }
    }

    private void SpawnBoss()
    {
        if (bossObjects == null || bossObjects.Length == 0 || bossSpawnPositions == null || bossSpawnPositions.Length == 0)
        {
            Debug.LogWarning("Boss spawn settings are not properly configured.");
            return;
        }

        int remainingBosses = bossObjects.Length - bossSpawnCount;
        int toSpawn = Mathf.Min(remainingBosses, bossesAtATime);

        if (remainingBosses <= 0)
        {
            bossSpawned = true;
            return;
        }

        for (int i = 0; i < toSpawn; i++)
        {
            Transform spawnPos = bossSpawnPositions[Random.Range(0, bossSpawnPositions.Length)];
            GameObject bossPrefab = bossObjects[Random.Range(0, bossObjects.Length)];
            Instantiate(bossPrefab, spawnPos.position, spawnPos.rotation);
            bossSpawnCount++;
        }

        if (remainingBosses <= 0)
            bossSpawned = true;
    }
    private void TryBoss(float delta)
    {
        bossTimer += delta;
        if (bossTimer >= bossSpawnRate) // Check every second
        {
            bossTimer = 0f;
            SpawnBoss();
        }

    }
    private float GetGroupSpawnProgress(SpawnGroup group)
    {
        if (group == null || group.count == 0) return 0;
        return (float)group.spawnedCount / group.count;
    }

    private float GetOverallSpawnProgress()
    {
        int totalToSpawn = 0;
        int totalSpawned = 0;
        SpawnGroup[] groups = { mainGroup, secondaryGroup, tertiaryGroup, quaternaryGroup };
        foreach (var group in groups)
        {
            if (group != null)
            {
                totalToSpawn += group.count;
                totalSpawned += group.spawnedCount;
            }
        }
        if (totalToSpawn == 0) return 0;

        return (float)totalSpawned / totalToSpawn;
    }

    private void SpawnGroupObjects(SpawnGroup group)
    {
        int remaining = group.count - group.spawnedCount;
        int toSpawn = Mathf.Min(remaining, group.spawnsPerInterval); // spawn 1 per interval


        for (int i = 0; i < toSpawn; i++)
        {
            Transform spawnPos = group.spawnPositions[Random.Range(0, group.spawnPositions.Length)];
            GameObject objToSpawn = group.objects[Random.Range(0, group.objects.Length)];
            Instantiate(objToSpawn, spawnPos.position, spawnPos.rotation);
            group.spawnedCount++;
        }
    }

    public void SpawnNow()
    {
        float delta = Time.deltaTime;

        if (autoSpawn)
        {
            TryAutoSpawn(delta);
        }
        else
        {
            TrySpawnGroup(mainGroup, delta);
            TrySpawnGroup(secondaryGroup, delta);
            TrySpawnGroup(tertiaryGroup, delta);
            TrySpawnGroup(quaternaryGroup, delta);
        }
        if (isBossSpawner && !bossSpawned && GetOverallSpawnProgress() >= spawnAtCompletionProgess)
        {
            TryBoss(delta);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
