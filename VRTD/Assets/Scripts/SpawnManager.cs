using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class SpawnManager : MonoBehaviour
{
    int ENEMY_COUNT_INCREASE = 3;
    float ENEMY_DELAY_FACTOR = 5;
    public GameObject enemy;
    public Vector3[] SpawnPoints;
    public float SpawnDelay;
    private float spawnTimer;
    public int EnemyCount;
    public int maxAtOnce;
    public int RoundNumber;
    private int num_Spawned;
    private int aliveNow;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateValues();
        maxAtOnce = 10;
    }

    // Update is called once per frame
    void Update()
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
        Transform myInstance = PoolManager.Pools["Enemies"].Spawn(enemy, SpawnPoints[idx], Quaternion.identity);
        //Debug.Log("Zombie spawned at " + myInstance);
        num_Spawned++;
        aliveNow++;
    }

    public void DecrementLiveZombies()
    {
        aliveNow--;
    }
}
