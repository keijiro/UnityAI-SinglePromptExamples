using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;

    [Header("Firing Settings")]
    public float fireRate = 0.8f;
    public int bulletsPerShot = 3;
    public float fanAngle = 40f;
    public float fireSpread = 15f;
    private float lastFireTime;

    private Transform player;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        MoveToPlayer();
        TryFire();
    }

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        
        // Rotate towards player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void TryFire()
    {
        if (Time.time - lastFireTime > fireRate)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        
        // Apply group spread (offset the whole fan)
        float groupOffset = Random.Range(-fireSpread, fireSpread);
        float centerAngle = baseAngle + groupOffset;

        // Calculate start angle for the fan
        float startAngle = centerAngle - fanAngle / 2f;
        float angleStep = (bulletsPerShot > 1) ? fanAngle / (bulletsPerShot - 1) : 0;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector2 bulletDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.SetDirection(bulletDirection);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ReflectedBullet") || other.CompareTag("Player"))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        // Only destroy if it has been spawned and moved at least a little bit
        // or just destroy it to be safe. Since they spawn outside, we need to be careful
        // they don't get destroyed immediately.
        // Better: Destroy if far enough from center or player.
        float distFromCenter = transform.position.magnitude;
        if (distFromCenter > 30f) // Assuming spawn radius is around 20-25
        {
            Destroy(gameObject);
        }
    }
}
