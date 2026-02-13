using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int sheepSaved = 0;
    public Text scoreText; // Optional: I'll create a simple UI for this

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore()
    {
        sheepSaved++;
        Debug.Log("Sheep Saved: " + sheepSaved);
        if (scoreText != null)
        {
            scoreText.text = "Saved: " + sheepSaved;
        }
    }
}
