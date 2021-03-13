using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleClicker : MonoBehaviour
{
    private KeyCode[] keys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Space };

    private Camera cam = null;

    private PuzzleElement selectedPuzzle = null;

    private PuzzleGenerator puzzleGenerator = null;

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
    }

    private void Click()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.TryGetComponent(out PuzzleElement puzzle))
            {
                if (selectedPuzzle)
                {
                    if (selectedPuzzle != puzzle)
                    {
                        var a = selectedPuzzle.GetPosition();
                        var b = puzzle.GetPosition();

                        selectedPuzzle.SetPosition(b);
                        puzzle.SetPosition(a);

                        if (puzzleGenerator.ValidatePuzzles())
                        {
                            Debug.Log("Success!");
                        }
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

    private void OnDrawGizmos()
    {
        if (selectedPuzzle == null) return;

        Gizmos.DrawSphere(selectedPuzzle.transform.position, 0.5f);
    }
}
