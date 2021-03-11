﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleClicker : MonoBehaviour
{
    private KeyCode[] keys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Space };

    private Camera cam = null;

    private PuzzleElement selectedPuzzle = null;

    private void Start()
    {
        cam = Camera.main;
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
                    var pos = selectedPuzzle.transform.position;
                    selectedPuzzle.transform.position = puzzle.transform.position;
                    puzzle.transform.position = pos;

                    selectedPuzzle = null;
                }
                else
                {
                    selectedPuzzle = puzzle;
                }
            }
            else
            {
                selectedPuzzle = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (selectedPuzzle == null) return;

        Gizmos.DrawSphere(selectedPuzzle.transform.position, 0.5f);
    }
}