using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public bool isReflected = false;
    private Vector2 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        rb.linearVelocity = direction * speed;
        // Rotate bullet to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Reflect(Vector2 normal)
    {
        if (isReflected) return;

        isReflected = true;
        direction = Vector2.Reflect(direction, normal);
        speed *= 2f;
        rb.linearVelocity = direction * speed;
        
        // Update rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Change color or something to indicate reflection? 
        // For now just change layer or tag if needed, but the logic handles 'isReflected'.
        gameObject.tag = "ReflectedBullet";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isReflected && other.CompareTag("Player"))
        {
            // Player death logic handled in PlayerController or GameManager
            Destroy(gameObject);
        }
        else if (isReflected && other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
