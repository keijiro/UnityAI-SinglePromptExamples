using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Static, Jumping }
    public EnemyType enemyType;
    
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpInterval = 2f;
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody2D rb;
    private float jumpTimer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpTimer = jumpInterval;
    }

    void Update()
    {
        // Move left using global scroll speed
        float currentSpeed = GameManager.Instance != null ? GameManager.Instance.ScrollSpeed : 5f;
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime, Space.World);

        if (enemyType == EnemyType.Jumping && isGrounded)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                Jump();
                jumpTimer = jumpInterval;
            }
        }

        // Destroy if off screen
        if (transform.position.x < -15f)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SubtractScore(200); // Penalty for missing an enemy
            }
            Destroy(gameObject);
        }
    }

    private void Jump()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit by player's sword
        if (other.CompareTag("Sword"))
        {
            Die();
        }
        else if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
        }
        Destroy(gameObject);
    }
}
