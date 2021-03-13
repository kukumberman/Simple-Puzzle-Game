using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElement : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter = null;

    private Vector3 randomPosition = Vector3.zero;

    private float rectSize = 0;

    private List<Vector3> vertices = new List<Vector3>();

    private BackgroundElement bg = null;

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

    //private void Update()
    //{
    //    meshFilter.mesh.uv = GetUV();
    //}

    public void Init(Vector3 position, Vector3 randomPos, float size, BackgroundElement background)
    {
        transform.position = position;

        randomPosition = randomPos;

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

        Mesh mesh = new Mesh();

        mesh.SetVertices(vertices);

        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        //mesh.triangles = new int[] { 0, 3, 1, 3, 0, 2 }; // default triangles in quad

        List<Vector2[]> uvs = new List<Vector2[]>
        {
            new Vector2[]
            {
                Vector2.zero,
                new Vector2(1, 0),
                new Vector2(0, 1),
                Vector2.one
            },
            new Vector2[]
            {
                Vector2.zero,
                new Vector2(0, 1),
                new Vector2(1, 0),
                Vector2.one
            }
        };
        //mesh.uv = uvs[Random.Range(0, uvs.Count)];

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

        Vector3 self = randomPosition;

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
