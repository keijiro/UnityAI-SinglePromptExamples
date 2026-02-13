using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float speed = 2f;
    public float avoidanceSpeed = 4f;
    public float avoidanceRange = 3f;
    public float minX = -9f;
    public float maxX = 9f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        Vector2 movement = Vector2.up * speed * Time.deltaTime;

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < avoidanceRange)
            {
                Vector2 avoidanceDir = (Vector2)transform.position - (Vector2)player.position;
                movement += avoidanceDir.normalized * avoidanceSpeed * Time.deltaTime;
            }
        }

        transform.Translate(movement, Space.World);

        // Clamping horizontal movement
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Cleanup
        if (transform.position.y > 6f) // Screen top roughly
        {
            if (GameManager.Instance != null) GameManager.Instance.AddScore();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wolf"))
        {
            // Sheep is eaten
            Destroy(gameObject);
        }
    }
}
