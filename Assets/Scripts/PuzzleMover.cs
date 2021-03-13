using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMover : MonoBehaviour
{
    [SerializeField] private bool isImmediately = true;
    [SerializeField] private float duration = 1;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isMoving = false;
    private float percentage01 = 0;
    private List<PuzzleElement> movingPuzzles = new List<PuzzleElement>();

    private PuzzleGenerator puzzleGenerator = null;

    private void Start()
    {
        puzzleGenerator = FindObjectOfType<PuzzleGenerator>();
    }

    private void Update()
    {
        OnUpdateMovePuzzles();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            puzzleGenerator.Finish();
        }
    }

    private void OnUpdateMovePuzzles()
    {
        if (isMoving)
        {
            percentage01 += 1f / duration * Time.deltaTime;

            float smoothPercent01 = animationCurve.Evaluate(percentage01);

            for (int i = 0; i < movingPuzzles.Count; i++)
            {
                var p = movingPuzzles[i];
                p.LerpTowards(smoothPercent01);
            }

            if (percentage01 >= 1)
            {
                for (int i = 0; i < movingPuzzles.Count; i++)
                {
                    var p = movingPuzzles[i];
                    p.SetTarget();
                }

                if (puzzleGenerator.ValidatePuzzles())
                {
                    Debug.Log("Success! (smooth)");
                }

                isMoving = false;
            }
        }
    }

    public void MovePuzzles(PuzzleElement p1, PuzzleElement p2)
    {
        Vector2Int a = p1.GetPosition();
        Vector2Int b = p2.GetPosition();

        if (isImmediately)
        {
            p1.SetPosition(b);
            p2.SetPosition(a);

            if (puzzleGenerator.ValidatePuzzles())
            {
                Debug.Log("Success! (isImmediately)");
            }
        }
        else
        {
            p1.SetSmoothTarget(b);
            p2.SetSmoothTarget(a);

            movingPuzzles.Clear();
            movingPuzzles.Add(p1);
            movingPuzzles.Add(p2);

            isMoving = true;
            percentage01 = 0;
        }
    }
}
