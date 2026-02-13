using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject sheepPrefab;
    public GameObject wolfPrefab;

    public float sheepSpawnInterval = 1.0f;
    public float wolfSpawnInterval = 2.0f;

    public float spawnRangeX = 8f;
    public float spawnY_Sheep = -5.5f;
    public float spawnY_Wolf = 5.5f;

    private float sheepTimer;
    private float wolfTimer;

    void Update()
    {
        sheepTimer += Time.deltaTime;
        if (sheepTimer >= sheepSpawnInterval)
        {
            Spawn(sheepPrefab, spawnY_Sheep);
            sheepTimer = 0;
        }

        wolfTimer += Time.deltaTime;
        if (wolfTimer >= wolfSpawnInterval)
        {
            Spawn(wolfPrefab, spawnY_Wolf);
            wolfTimer = 0;
        }
    }

    void Spawn(GameObject prefab, float yPos)
    {
        if (prefab == null) return;
        float x = Random.Range(-spawnRangeX, spawnRangeX);
        Instantiate(prefab, new Vector3(x, yPos, 0), Quaternion.identity);
    }
}
