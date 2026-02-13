using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10f;
    public float smoothTime = 0.1f;
    private Vector2 velocity;

    [Header("Energy")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyConsumptionRate = 30f;
    public float energyRecoveryRate = 20f;

    [Header("Shield")]
    public GameObject shieldObject;
    public float shieldRadius = 1.5f;

    [Header("State")]
    private bool isDead = false;

    void Start()
    {
        currentEnergy = maxEnergy;
        if (shieldObject) shieldObject.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleShield();
        HandleEnergy();
    }

    void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector2 targetPos = mousePos;
        Vector2 currentPos = transform.position;
        
        Vector2 newPos = Vector2.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime, maxSpeed);
        transform.position = newPos;

        // Rotation towards movement
        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 10f * Time.deltaTime);
        }
    }

    void HandleShield()
    {
        bool shieldRequested = Input.GetMouseButton(0) && currentEnergy > 0;
        
        if (shieldObject)
        {
            shieldObject.SetActive(shieldRequested);
        }

        if (shieldRequested)
        {
            currentEnergy -= energyConsumptionRate * Time.deltaTime;
            currentEnergy = Mathf.Max(0, currentEnergy);

            // Reflection Logic (SphereCast or checking nearby bullets)
            Collider2D[] bullets = Physics2D.OverlapCircleAll(transform.position, shieldRadius);
            foreach (var col in bullets)
            {
                Bullet b = col.GetComponent<Bullet>();
                if (b != null && !b.isReflected)
                {
                    Vector2 normal = (b.transform.position - transform.position).normalized;
                    b.Reflect(normal);
                }
            }
        }
    }

    void HandleEnergy()
    {
        bool isShielding = shieldObject != null && shieldObject.activeSelf;
        bool isStationary = velocity.magnitude < 0.1f;

        if (!isShielding && isStationary)
        {
            currentEnergy += energyRecoveryRate * Time.deltaTime;
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("EnemyBullet") || collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    [Header("Effects")]
    public GameObject explosionPrefab;

    public void Explode()
    {
        if (isDead) return;
        isDead = true;
        
        if (explosionPrefab)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log("Player Exploded!");
        GetComponent<SpriteRenderer>().enabled = false;
        if (shieldObject) shieldObject.SetActive(false);
        
        Invoke("RestartGame", 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
