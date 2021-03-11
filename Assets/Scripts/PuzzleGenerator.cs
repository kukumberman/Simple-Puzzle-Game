using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize = Vector2Int.one;
    [SerializeField] private float chunkSize = 3;
    [SerializeField] private float spacing = 1;
    [SerializeField] private PuzzleElement puzzleElementPrefab = null;

    private BackgroundElement bg = null;

    private void Start()
    {
        bg = FindObjectOfType<BackgroundElement>();

        List<Vector3> positions = new List<Vector3>();

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 pos = GetPosition(x, y) + transform.position;
                positions.Add(pos);
            }
        }

        Queue<Vector3> queue = new Queue<Vector3>(positions);

        for (int i = 0, length = gridSize.x * gridSize.y; i < length; i++)
        {
            int randomIndex = Random.Range(0, positions.Count);
            Vector3 randomPosition = positions[randomIndex];
            positions.RemoveAt(randomIndex);

            Vector3 pos = queue.Dequeue();

            var puzzle = Instantiate(puzzleElementPrefab, transform);
            puzzle.Init(pos, randomPosition, chunkSize, bg);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 size = Vector2.one * chunkSize;

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 pos = GetPosition(x, y) + transform.position;

                Gizmos.DrawWireCube(pos, size);
            }
        }
    }

    private Vector3 GetPosition(int x, int y)
    {
        Vector2 xy = new Vector2(x, y);

        Vector2 centerOffset = Vector2.one * 0.5f;

        Vector2 gridOffset = new Vector2(-gridSize.x * 0.5f, -gridSize.y * 0.5f);

        Vector3 pos = gridOffset + xy + centerOffset;

        Vector3 position = pos * chunkSize + pos * spacing;

        return position;
    }
}
