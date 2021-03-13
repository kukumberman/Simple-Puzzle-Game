using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleClicker : MonoBehaviour
{
    [SerializeField] private bool isImmediately = true;
    [SerializeField] private float duration = 2;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private KeyCode[] keys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Space };

    private Camera cam = null;

    private PuzzleElement selectedPuzzle = null;

    private PuzzleGenerator puzzleGenerator = null;

    private bool isMoving = false;
    private float percentage01 = 0;
    private List<PuzzleElement> movingPuzzles = new List<PuzzleElement>();

    private void Start()
    {
        cam = Camera.main;

        puzzleGenerator = FindObjectOfType<PuzzleGenerator>();
    }

    private void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                Click();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            puzzleGenerator.Finish();
        }

        OnUpdateMovePuzzles();
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

    private void Click()
    {
        if (isMoving) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.TryGetComponent(out PuzzleElement puzzle))
            {
                if (selectedPuzzle)
                {
                    if (selectedPuzzle != puzzle)
                    {
                        MovePuzzles(selectedPuzzle, puzzle, isImmediately);
                    }
                    else
                    {
                        Debug.Log("clicked same");
                    }

                    Deselect();
                }
                else
                {
                    selectedPuzzle = puzzle;
                    selectedPuzzle.Outline(true);
                }
            }
        }
        else
        {
            Deselect();
        }
    }

    private void Deselect()
    {
        selectedPuzzle?.Outline(false);
        selectedPuzzle = null;
    }

    private void MovePuzzles(PuzzleElement p1, PuzzleElement p2, bool isImmediately)
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

    private void OnDrawGizmos()
    {
        if (selectedPuzzle == null) return;

        Gizmos.DrawSphere(selectedPuzzle.transform.position, 0.5f);
    }
}
