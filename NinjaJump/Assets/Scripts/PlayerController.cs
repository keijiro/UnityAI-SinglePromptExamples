using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private GameObject swordVisual;
    [SerializeField] private float backflipSpeed = 720f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (swordVisual != null) swordVisual.SetActive(false);
    }

    void Update()
    {
        HandleSpeedInput();

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            Jump();
        }

        if (isJumping)
        {
            // Backflip rotation
            transform.Rotate(Vector3.forward, -backflipSpeed * Time.deltaTime);
        }
    }

    private void HandleSpeedInput()
    {
        float speedMultiplier = 1f;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            speedMultiplier = 1.5f; // Faster
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            speedMultiplier = 0.6f; // Slower
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSpeedMultiplier(speedMultiplier);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        isJumping = true;
        if (swordVisual != null) swordVisual.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            transform.rotation = Quaternion.identity;
            if (swordVisual != null) swordVisual.SetActive(false);
        }
    }
}
