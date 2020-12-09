using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class SpawnManager : MonoBehaviour
{
    int ENEMY_COUNT_INCREASE = 3;
    float ENEMY_DELAY_FACTOR = 5;
    public GameObject enemy;
    public GameObject enemy2;
    private List<GameObject> spawnables = new List<GameObject>();
    public Vector3[] SpawnPoints;
    public float SpawnDelay;
    private float spawnTimer;
    public int EnemyCount;
    public int maxAtOnce;
    public int RoundNumber;
    private int num_Spawned;
    public int aliveNow;
    private bool SpawnEnabled = false;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnables.Add(enemy);
        UpdateValues();
        maxAtOnce = 15;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnEnabled)
        {
            spawnTimer += Time.deltaTime;

            if (num_Spawned == EnemyCount && aliveNow == 0)
            {
                Debug.Log("Round " + RoundNumber + " complete. ");
                IncrementRound();
            }

            if (spawnTimer >= SpawnDelay && num_Spawned < EnemyCount && aliveNow < maxAtOnce)
            {
                SpawnZombie();
                spawnTimer = 0f;
            }
        }
    }

    public void IncrementRound()
    {
        RoundNumber++;
        UpdateValues();
    }

    void UpdateValues()
    {
        EnemyCount = RoundNumber * ENEMY_COUNT_INCREASE;
        SpawnDelay = ENEMY_DELAY_FACTOR / RoundNumber;
        num_Spawned = 0;
        aliveNow = 0;
    }

    void SpawnZombie()
    {
        int idx = Random.Range(0, SpawnPoints.Length-1);
        int idx2 = idx - 1;
        if (idx2 < 0)
        {
            idx2 = SpawnPoints.Length - 1;
        }

        // Get random enemy from spawnable enemies
        int enemy_idx = Random.Range(0, spawnables.Count);
        //enemy = spawnables[enemy_idx];
        float spawn_x = Random.Range(SpawnPoints[idx].x, SpawnPoints[idx2].x);
        float spawn_z = Random.Range(SpawnPoints[idx].z, SpawnPoints[idx2].z);
        Transform myInstance = PoolManager.Pools["Enemies"].Spawn(spawnables[enemy_idx], new Vector3(spawn_x, SpawnPoints[0].y, spawn_z), Quaternion.identity);
        num_Spawned++;
        aliveNow++;
    }

    public void DecrementLiveZombies()
    {
        aliveNow--;
    }

    // Get remaining enemies for cooldown transition
    public int GetNumAlive()
    {
        return aliveNow;
    }

    // Enable/disable spawning
    public void SetSpawnEnabled(bool enabled)
    {
        SpawnEnabled = enabled;
    }

    // Check if a GameObject is in the spawnables list
    private bool CheckSpawnable(GameObject objToCheck)
    {
        foreach (GameObject spawnable in spawnables)
        {
            if (spawnable.GetType().Equals(objToCheck))
            {
                return true;
            }
        }
        return false;
    }

    // Add enemy to spawnable list
    public bool AddEnemyToSpawns(GameObject enemyToAdd)
    {
        // Only add if the enemy isn't already in the list
        if (!CheckSpawnable(enemyToAdd))
        {
            spawnables.Add(enemyToAdd);
            return true;
        }
        return false;
    }

    // Remove enemy from spawnable list
    public bool RemoveEnemyFromSpawns(GameObject enemyToRemove)
    {
        return spawnables.Remove(enemyToRemove); // Return the success of removing the enemy
    }
}
