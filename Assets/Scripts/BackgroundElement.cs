using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundElement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;

    public Vector2[] GetCorners()
    {
        Vector3 bottomLeft = spriteRenderer.bounds.min;
        Vector3 topRight = spriteRenderer.bounds.max;

        Vector2[] corners = new Vector2[]
        {
            bottomLeft,
            new Vector2(topRight.x, bottomLeft.y),
            new Vector2(bottomLeft.x, topRight.y),
            topRight
        };

        return corners;
    }

    private void OnDrawGizmos()
    {
        if (spriteRenderer == null) return;

        float radius = 0.5f;

        //Vector3 min = spriteRenderer.bounds.min;
        //Vector3 max = spriteRenderer.bounds.max;

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(min, radius);

        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(max, radius);

        var corners = GetCorners();

        Color a = Color.blue;
        Color b = Color.red;

        for (int i = 0, length = corners.Length; i < length; i++)
        {
            float t = (float)i / (length - 1);
            Gizmos.color = Color.Lerp(a, b, t);

            Gizmos.DrawSphere(corners[i], radius);
        }
    }
}
