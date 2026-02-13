using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    public GameObject bulletPrefab;
    public float fireRate = 1.5f;
    public int bulletsPerBurst = 5;
    public float spreadAngle = 30f;

    private Transform player;
    private float fireTimer;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        fireTimer = Random.Range(0, fireRate);
    }

    void Update()
    {
        if (player == null) return;

        // Move towards player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Rotate towards player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Shooting
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        if (player == null) return;

        Vector2 baseDir = (player.position - transform.position).normalized;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            float angleOffset = Random.Range(-spreadAngle, spreadAngle);
            Vector2 shootDir = Quaternion.Euler(0, 0, angleOffset) * baseDir;

            GameObject bObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bObj.tag = "EnemyBullet";
            Bullet b = bObj.GetComponent<Bullet>();
            if (b)
            {
                b.SetDirection(shootDir);
            }
        }
    }
}
