using UnityEngine;

public class SheepController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float avoidanceSpeed = 4f;
    public float avoidanceRadius = 3f;
    
    private GameManager gameManager;

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        Vector2 movement = Vector2.up * moveSpeed * Time.deltaTime;

        // Avoidance
        GameObject[] corgis = GameObject.FindGameObjectsWithTag("Corgi");
        foreach (var corgi in corgis)
        {
            float dist = Vector2.Distance(transform.position, corgi.transform.position);
            if (dist < avoidanceRadius)
            {
                Vector2 awayDir = (Vector2)(transform.position - corgi.transform.position).normalized;
                movement += awayDir * avoidanceSpeed * Time.deltaTime;
            }
        }

        transform.Translate(movement);

        // Cleanup if way off screen
        if (transform.position.y > 10f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wolf"))
        {
            gameManager.AddScore(-1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Hut"))
        {
            gameManager.AddScore(1);
            Destroy(gameObject);
        }
    }
}
