using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [SerializeField] private List<Spawner> spawnerStack;
    public List<Spawner> SpawnerStack => spawnerStack;
    [SerializeField] private AdvancedPlayerController playerController;


    public bool isSpawning = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateSpawner(Spawner spawner)
    {
        spawner.GetComponentInChildren<Spawner>().enabled = true;

    }

    public void DeactivateSpawner(Spawner spawner)
    {
        spawner.GetComponentInChildren<Spawner>().enabled = false;
    }

    public void ClearSpawners()
    {
        foreach (Spawner spawner in spawnerStack)
        {
            spawner.GetComponentInChildren<Spawner>().enabled = false;
        }
        // remove current spawners from the stack
        spawnerStack[0] = null;
    }
}
