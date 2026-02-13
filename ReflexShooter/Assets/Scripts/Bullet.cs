using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public bool isReflected = false;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        UpdateRotation();
        ApplyVelocity();
    }

    private void UpdateRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Add TrailRenderer for Sci-Fi look
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.2f;
        trail.startWidth = 0.1f;
        trail.endWidth = 0.0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = GetComponent<SpriteRenderer>().color;
        trail.endColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0);
        
        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void Update()
    {
        // Simple cleanup if far away
        if (transform.position.sqrMagnitude > 10000f)
        {
            Destroy(gameObject);
        }
    }

    public void Reflect(Vector2 normal)
    {
        if (isReflected) return;

        isReflected = true;
        direction = Vector2.Reflect(direction, normal).normalized;
        speed *= 2f;
        UpdateRotation();
        ApplyVelocity();
        
        // Change color to indicate it's dangerous to enemies
        Color reflectedColor = Color.cyan;
        GetComponent<SpriteRenderer>().color = reflectedColor;
        
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail)
        {
            trail.startColor = reflectedColor;
            trail.endColor = new Color(reflectedColor.r, reflectedColor.g, reflectedColor.b, 0);
        }
    }

    public GameObject explosionPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isReflected && collision.CompareTag("Player"))
        {
            PlayerController p = collision.GetComponent<PlayerController>();
            if (p != null) p.Explode();
            Destroy(gameObject);
        }
        else if (isReflected && collision.CompareTag("Enemy"))
        {
            if (explosionPrefab)
            {
                Instantiate(explosionPrefab, collision.transform.position, Quaternion.identity);
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
