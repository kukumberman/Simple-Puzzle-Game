using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoTester : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter = null;

    private void Start()
    {
        string uv = string.Join(", ", meshFilter.mesh.uv);
        string t = string.Join(", ", meshFilter.mesh.triangles);

        Debug.Log(uv);
        Debug.Log(t);
    }
}
