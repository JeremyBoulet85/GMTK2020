using UnityEngine;

public static class GameObjectEx
{
    public static void DrawCircle(this GameObject container, float radius, float lineWidth, Color color)
    {
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;
        line.material.SetColor("_TintColor", new Color(1, 1, 1, 0.1f));

        var pointCount = segments + 1;
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, -10.0f);
        }

        line.SetPositions(points);
    }
}