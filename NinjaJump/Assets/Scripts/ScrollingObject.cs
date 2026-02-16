using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    [SerializeField] private float width = 20f;

    void Update()
    {
        float currentSpeed = GameManager.Instance != null ? GameManager.Instance.ScrollSpeed : 5f;
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        if (transform.position.x <= -width)
        {
            transform.position += new Vector3(width * 2, 0, 0);
        }
    }
}
