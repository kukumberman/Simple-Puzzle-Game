using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MeshFilter meshFilter = null;
    [SerializeField] private MeshRenderer meshRenderer = null;

    [Header("Properties")]
    [SerializeField] private string outlinePropertyName = "_UseGradient";

    private Vector2Int gridCurrent = Vector2Int.zero;
    private Vector2Int gridTarget = Vector2Int.zero;

    private float rectSize = 0;

    private List<Vector3> vertices = new List<Vector3>();

    private PuzzleGenerator puzzleGenerator = null;
    private BackgroundElement bg = null;

    private int propertyId = 0;

    private Vector2Int smoothGridTarget = Vector2Int.zero;
    private Vector3 smoothTarget = Vector3.zero;
    private Vector3 smoothCurrent = Vector3.zero;

    private void OnDrawGizmos()
    {
        Color a = Color.blue;
        Color b = Color.red;

        for (int i = 0, length = vertices.Count; i < length; i++)
        {
            float t = (float)i / (length - 1);
            Gizmos.color = Color.Lerp(a, b, t);

            Gizmos.DrawSphere(vertices[i] + transform.position, 0.2f);
        }
    }

    public void Outline(bool active)
    {
        //if (IsCorrectPosition()) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        meshRenderer.GetPropertyBlock(mpb);

        int value = active ? 1 : 0;

        mpb.SetInt(propertyId, value);

        meshRenderer.SetPropertyBlock(mpb);
    }

    public void DisableOutline()
    {
        // to do: remove white outline
    }

    public bool IsCorrectPosition()
    {
        return gridCurrent == gridTarget;
    }

    public Vector2Int GetPosition()
    {
        return gridCurrent;
    }

    public void SetPosition(Vector2Int grid)
    {
        gridCurrent = grid;

        transform.position = puzzleGenerator.GetPosition(gridCurrent);
    }

    public void SetCorrectPosition()
    {
        gridCurrent = gridTarget;

        transform.position = puzzleGenerator.GetPosition(gridCurrent);
    }

    //

    public void SetSmoothTarget(Vector2Int target)
    {
        smoothGridTarget = target;
        smoothTarget = puzzleGenerator.GetPosition(target);
        smoothCurrent = puzzleGenerator.GetPosition(gridCurrent);
    }

    public void SetTarget()
    {
        SetPosition(smoothGridTarget);
    }

    public void LerpTowards(float percentage01)
    {
        //Vector3 a = puzzleGenerator.GetPosition(gridCurrent);

        Vector3 pos = Vector3.Lerp(smoothCurrent, smoothTarget, percentage01);

        transform.position = pos;
    }

    //

    public void Init(PuzzleGenerator generator, Vector2Int gridCurrentPosition, Vector2Int gridTargetPosition, float size, BackgroundElement background)
    {
        puzzleGenerator = generator;

        // positions

        transform.position = puzzleGenerator.GetPosition(gridCurrentPosition);

        gridCurrent = gridCurrentPosition;
        gridTarget = gridTargetPosition;

        if (IsCorrectPosition())
        {
            DisableOutline();
        }

        // mesh

        rectSize = size;
        bg = background;

        GetComponent<BoxCollider>().size = Vector2.one * size;

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                Vector3 pos = GetPosition(x, y);
                vertices.Add(pos);
            }
        }

        GenerateMesh();

        propertyId = Shader.PropertyToID(outlinePropertyName);
    }

    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        mesh.SetVertices(vertices);

        // 0, 3, 1, 3, 0, 2 = default triangles in quad
        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };

        var uv = GetUV();

        //Debug.Log(string.Join(" ", uv));

        mesh.SetUVs(0, uv);
        mesh.SetUVs(1, GetDefaultUV());

        meshFilter.mesh = mesh;
    }

    private Vector3 GetPosition(int x, int y)
    {
        Vector3 offset = Vector2.one * rectSize;

        Vector3 pos = new Vector2(x, y) + Vector2.one * 0.5f;

        pos *= rectSize;

        pos -= offset;

        return pos;
    }

    private Vector2[] GetUV()
    {
        Vector2[] uv = new Vector2[4];

        Vector2[] corners = bg.GetCorners();

        Vector3 self = puzzleGenerator.GetPosition(gridTarget);

        float minX = Mathf.InverseLerp(corners[0].x, corners[1].x, vertices[0].x + self.x);
        float maxX = Mathf.InverseLerp(corners[0].x, corners[1].x, vertices[1].x + self.x);

        float minY = Mathf.InverseLerp(corners[0].y, corners[2].y, vertices[0].y + self.y);
        float maxY = Mathf.InverseLerp(corners[0].y, corners[2].y, vertices[3].y + self.y);

        uv[0] = new Vector2(minX, minY);
        uv[1] = new Vector2(maxX, minY);
        uv[2] = new Vector2(minX, maxY);
        uv[3] = new Vector2(maxX, maxY);

        return uv;
    }
    
    private Vector2[] GetDefaultUV()
    {
        Vector2[] defaultUV = new Vector2[]
        {
            Vector2.zero,
            new Vector2(1, 0),
            new Vector2(0, 1),
            Vector2.one
        };

        return defaultUV;
    }
}
