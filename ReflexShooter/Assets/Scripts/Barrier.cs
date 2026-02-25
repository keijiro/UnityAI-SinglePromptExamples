using UnityEngine;

public class Barrier : MonoBehaviour
{
    private Collider2D barrierCollider;

    void Awake()
    {
        barrierCollider = GetComponent<Collider2D>();
        if (barrierCollider == null) barrierCollider = gameObject.AddComponent<CircleCollider2D>();
        barrierCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null && !bullet.isReflected)
            {
                // Calculate normal for reflection (from barrier center to bullet)
                Vector2 normal = (other.transform.position - transform.position).normalized;
                bullet.Reflect(normal);
            }
        }
    }
}
