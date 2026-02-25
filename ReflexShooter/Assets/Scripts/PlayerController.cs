using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float rotationSpeed = 180f;

    [Header("Energy & Barrier")]
    public GameObject barrier;
    public float maxEnergy = 100f;
    public float energyConsumption = 30f;
    public float energyRecovery = 15f;
    private float currentEnergy;
    private bool isBarrierActive = false;

    [Header("Death")]
    public GameObject explosionPrefab;
    private bool isDead = false;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearDamping = 5f;

        currentEnergy = maxEnergy;
        if (barrier != null) barrier.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        HandleBarrier();
        HandleEnergy();
        RotateToCursor();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        MoveToCursor();
    }

    void MoveToCursor()
    {
        // Calculate target world position on Z=0 plane
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z); 
        Vector2 targetWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        Vector2 direction = (targetWorldPos - (Vector2)transform.position);
        float distance = direction.magnitude;

        // Calculate desired velocity
        // It slows down as it gets closer to the mouse
        float speed = Mathf.Min(maxSpeed, distance * 5f); 
        Vector2 desiredVelocity = direction.normalized * speed;

        // Use a more stable acceleration logic
        // We limit the force so it doesn't cause physics explosions
        Vector2 velocityDiff = desiredVelocity - rb.linearVelocity;
        Vector2 force = velocityDiff * acceleration;

        // Limit maximum force
        float maxForce = 200f;
        if (force.magnitude > maxForce)
        {
            force = force.normalized * maxForce;
        }

        rb.AddForce(force);
    }

    void RotateToCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector2 targetWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        Vector2 direction = (targetWorldPos - (Vector2)transform.position);
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleBarrier()
    {
        bool mouseHeld = Input.GetMouseButton(0);
        if (mouseHeld && currentEnergy > 0)
        {
            if (!isBarrierActive)
            {
                isBarrierActive = true;
                if (barrier != null) barrier.SetActive(true);
            }
        }
        else
        {
            if (isBarrierActive)
            {
                isBarrierActive = false;
                if (barrier != null) barrier.SetActive(false);
            }
        }
    }

    void HandleEnergy()
    {
        if (isBarrierActive)
        {
            currentEnergy -= energyConsumption * Time.deltaTime;
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
                isBarrierActive = false;
                if (barrier != null) barrier.SetActive(false);
            }
        }
        else
        {
            // Recover when stationary (or very slow) and barrier is off
            if (rb.linearVelocity.magnitude < 0.5f)
            {
                currentEnergy += energyRecovery * Time.deltaTime;
                currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // Player is hit by enemy bullet (only if not reflected)
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null && !bullet.isReflected)
            {
                Die();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Destroyed!");
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        GetComponent<SpriteRenderer>().enabled = false;
        if (barrier != null) barrier.SetActive(false);
        
        // Disable physics
        rb.simulated = false;

        Invoke(nameof(RestartGame), 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GetEnergyNormalized() => currentEnergy / maxEnergy;
    public bool IsBarrierActive() => isBarrierActive;
}
