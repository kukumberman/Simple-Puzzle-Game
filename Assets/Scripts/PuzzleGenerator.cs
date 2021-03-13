using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize = Vector2Int.one;
    [SerializeField] private float chunkSize = 3;
    [SerializeField] private float spacing = 1;
    [SerializeField] private PuzzleElement puzzleElementPrefab = null;

    private PuzzleElement[] puzzles = null;

    private BackgroundElement bg = null;

    private void Start()
    {
        bg = FindObjectOfType<BackgroundElement>();

        List<Vector2Int> positions = new List<Vector2Int>();

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                positions.Add(new Vector2Int(x, y));
            }
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>(positions);

        int count = gridSize.x * gridSize.y;

        puzzles = new PuzzleElement[count];

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, positions.Count);
            Vector2Int randomPosition = positions[randomIndex];
            positions.RemoveAt(randomIndex);

            Vector2Int gridPosition = queue.Dequeue();

            var puzzle = Instantiate(puzzleElementPrefab, transform);
            puzzle.Init(this, gridPosition, randomPosition, chunkSize, bg);

            puzzles[i] = puzzle;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 size = Vector2.one * chunkSize;

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 pos = GetPosition(x, y);

                Gizmos.DrawWireCube(pos, size);
            }
        }
    }

    public void Finish()
    {
        for (int i = 0; i < puzzles.Length; i++)
        {
            var p = puzzles[i];

            if (!p.IsCorrectPosition())
            {
                p.SetCorrectPosition();
            }
        }

        if (ValidatePuzzles())
        {
            Debug.Log("ended by single click");
        }
    }

    public bool ValidatePuzzles()
    {
        bool isEnd = true;

        for (int i = 0; i < puzzles.Length; i++)
        {
            var p = puzzles[i];

            if (!p.IsCorrectPosition())
            {
                isEnd = false;
                break;
            }
        }

        if (isEnd)
        {
            DisableOutline();
        }

        return isEnd;
    }

    private void DisableOutline()
    {
        // todo: outline is not disable because of shader

        Debug.Log("is end");

        for (int i = 0; i < puzzles.Length; i++)
        {
            var p = puzzles[i];

            p.DisableOutline();
        }
    }

    public Vector3 GetPosition(Vector2Int grid)
    {
        return GetPosition(grid.x, grid.y);
    }

    private Vector3 GetPosition(int x, int y)
    {
        Vector2 xy = new Vector2(x, y);

        Vector2 centerOffset = Vector2.one * 0.5f;

        Vector2 gridOffset = new Vector2(-gridSize.x * 0.5f, -gridSize.y * 0.5f);

        Vector3 pos = gridOffset + xy + centerOffset;

        Vector3 position = pos * chunkSize + pos * spacing;

        return position + transform.position;
    }
}
