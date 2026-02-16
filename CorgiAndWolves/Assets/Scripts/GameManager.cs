using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject sheepPrefab;
    public GameObject wolfPrefab;
    public GameObject corgiPrefab;

    private int score = 0;
    private Label scoreLabel;

    void Start()
    {
        var uiDocument = Object.FindFirstObjectByType<UIDocument>();
        if (uiDocument != null)
        {
            scoreLabel = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        }

        SpawnCorgis();
        StartCoroutine(SheepSpawnRoutine());
        
        // Start independent routines for each wolf
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(SingleWolfRoutine());
        }
    }

    IEnumerator SingleWolfRoutine()
    {
        GameObject wolf = Instantiate(wolfPrefab);
        wolf.SetActive(false);

        while (true)
        {
            // Wait before appearing
            yield return new WaitForSeconds(Random.Range(2f, 7f));

            // Appear at random position
            wolf.transform.position = new Vector3(Random.Range(-6f, 6f), Random.Range(-2f, 4f), 0);
            wolf.SetActive(true);

            // Stay for a while
            yield return new WaitForSeconds(Random.Range(10f, 20f));

            // Disappear
            wolf.SetActive(false);
        }
    }

    void SpawnCorgis()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(corgiPrefab, new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0), Quaternion.identity);
        }
    }

    IEnumerator SheepSpawnRoutine()
    {
        while (true)
        {
            float spawnX = Random.Range(-6f, 6f);
            int count = Random.Range(10, 20);
            for (int i = 0; i < count; i++)
            {
                Instantiate(sheepPrefab, new Vector3(spawnX, -6f, 0), Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(5f);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreLabel != null)
        {
            scoreLabel.text = $"Score: {score}";
        }
    }
}
