using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float time = 0.5f;
    void Start()
    {
        Destroy(gameObject, time);
    }
}
