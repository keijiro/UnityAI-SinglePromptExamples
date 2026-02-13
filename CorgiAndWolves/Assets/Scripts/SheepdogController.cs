using UnityEngine;

public class SheepdogController : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.1f;
    private Vector2 currentVelocity;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        
        transform.position = Vector2.SmoothDamp(transform.position, mousePosition, ref currentVelocity, smoothTime);

        // Rotate to look at movement direction
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Adjusting for sprite orientation (assuming sprite faces up)
        }
    }
}
