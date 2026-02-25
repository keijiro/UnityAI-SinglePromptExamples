using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float spawnRadius = 15f;
    private float lastSpawnTime;

    void Start()
    {
        SpawnEnemy();
        lastSpawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lastSpawnTime > spawnRate)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
            
            // Gradually increase difficulty
            spawnRate = Mathf.Max(0.5f, spawnRate - 0.01f);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Spawn at random position on circle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 spawnPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
