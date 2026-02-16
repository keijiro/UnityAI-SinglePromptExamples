using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float maxSpawnInterval = 3.5f;
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float groundY = -3.5f;

    private float timer;
    private float nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextSpawnTime)
        {
            SpawnEnemy();
            timer = 0;
            SetNextSpawnTime();
        }
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = new Vector3(spawnX, groundY, 0);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            // Randomly choose type
            controller.enemyType = Random.value > 0.5f ? EnemyController.EnemyType.Static : EnemyController.EnemyType.Jumping;
        }
    }
}
