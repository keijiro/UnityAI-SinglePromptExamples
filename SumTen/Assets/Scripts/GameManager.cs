using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public int width = 6;
    public int height = 12;
    public float fallSpeed = 1f;
    public GameObject blockPrefab;

    [Header("State")]
    private Block[,] grid;
    private List<Block> activePair = new List<Block>();
    private bool isProcessing = false;
    private int score = 0;

    public System.Action<int> OnScoreChanged;
    public System.Action OnGameOver;

    private void Awake()
    {
        Instance = this;
        grid = new Block[width, height];
    }

    private void Start()
    {
        SpawnNewPair();
    }

    private void Update()
    {
        if (isProcessing || activePair.Count == 0) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveActivePair(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveActivePair(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Manual drop
            StartCoroutine(DropActivePair());
        }
    }

    public void SpawnNewPair()
    {
        int x1 = width / 2 - 1;
        int x2 = width / 2;
        int y = height - 1;

        if (grid[x1, y] != null || grid[x2, y] != null)
        {
            OnGameOver?.Invoke();
            return;
        }

        int val1, val2;
        do
        {
            val1 = Random.Range(1, 10);
            val2 = Random.Range(1, 10);
        } while (val1 + val2 == 10);

        Block b1 = CreateBlock(x1, y, val1);
        Block b2 = CreateBlock(x2, y, val2);

        activePair.Clear();
        activePair.Add(b1);
        activePair.Add(b2);

        StartCoroutine(FallRoutine());
    }

    private Block CreateBlock(int x, int y, int value)
    {
        GameObject go = Instantiate(blockPrefab, new Vector3(x, y, 0), Quaternion.identity);
        Block block = go.GetComponent<Block>();
        block.SetValue(value);
        return block;
    }

    private void MoveActivePair(Vector2Int direction)
    {
        bool canMove = true;
        foreach (var block in activePair)
        {
            int nextX = Mathf.RoundToInt(block.transform.position.x) + direction.x;
            int nextY = Mathf.RoundToInt(block.transform.position.y);

            if (nextX < 0 || nextX >= width || grid[nextX, nextY] != null)
            {
                canMove = false;
                break;
            }
        }

        if (canMove)
        {
            foreach (var block in activePair)
            {
                block.transform.position += (Vector3)(Vector2)direction;
            }
        }
    }

    private IEnumerator FallRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fallSpeed);
            if (isProcessing) yield break;

            if (!MoveDown())
            {
                yield return StartCoroutine(ProcessLandedBlocks());
                break;
            }
        }
    }

    private IEnumerator DropActivePair()
    {
        if (isProcessing) yield break;
        isProcessing = true;
        while (MoveDown())
        {
            yield return new WaitForSeconds(0.05f);
        }
        yield return StartCoroutine(ProcessLandedBlocks());
        isProcessing = false;
    }

    private bool MoveDown()
    {
        bool canMove = true;
        foreach (var block in activePair)
        {
            int nextX = Mathf.RoundToInt(block.transform.position.x);
            int nextY = Mathf.RoundToInt(block.transform.position.y) - 1;

            if (nextY < 0 || grid[nextX, nextY] != null)
            {
                canMove = false;
                break;
            }
        }

        if (canMove)
        {
            foreach (var block in activePair)
            {
                block.transform.position += Vector3.down;
            }
            return true;
        }
        return false;
    }

    private IEnumerator ProcessLandedBlocks()
    {
        isProcessing = true;

        // Register blocks into grid
        foreach (var block in activePair)
        {
            int x = Mathf.RoundToInt(block.transform.position.x);
            int y = Mathf.RoundToInt(block.transform.position.y);
            grid[x, y] = block;
        }
        activePair.Clear();

        bool changed = true;
        while (changed)
        {
            changed = false;
            
            // 1. Check for matches
            HashSet<Block> toRemove = new HashSet<Block>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == null) continue;

                    // Check horizontal
                    if (x + 1 < width && grid[x + 1, y] != null)
                    {
                        if (grid[x, y].Value + grid[x + 1, y].Value == 10)
                        {
                            toRemove.Add(grid[x, y]);
                            toRemove.Add(grid[x + 1, y]);
                        }
                    }
                    // Check vertical
                    if (y + 1 < height && grid[x, y + 1] != null)
                    {
                        if (grid[x, y].Value + grid[x, y + 1].Value == 10)
                        {
                            toRemove.Add(grid[x, y]);
                            toRemove.Add(grid[x, y + 1]);
                        }
                    }
                }
            }

            if (toRemove.Count > 0)
            {
                score += toRemove.Count * 10;
                OnScoreChanged?.Invoke(score);
                
                foreach (var block in toRemove)
                {
                    int x = Mathf.RoundToInt(block.transform.position.x);
                    int y = Mathf.RoundToInt(block.transform.position.y);
                    grid[x, y] = null;
                    Destroy(block.gameObject);
                }
                
                yield return new WaitForSeconds(0.3f);

                // 2. Apply gravity to all blocks
                bool dropped;
                do
                {
                    dropped = false;
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 1; y < height; y++)
                        {
                            if (grid[x, y] != null && grid[x, y - 1] == null)
                            {
                                grid[x, y - 1] = grid[x, y];
                                grid[x, y] = null;
                                grid[x, y - 1].transform.position += Vector3.down;
                                dropped = true;
                                changed = true;
                            }
                        }
                    }
                    if (dropped) yield return new WaitForSeconds(0.1f);
                } while (dropped);
            }
        }

        isProcessing = false;
        SpawnNewPair();
    }
}
