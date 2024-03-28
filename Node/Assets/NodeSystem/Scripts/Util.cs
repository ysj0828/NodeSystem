using UnityEngine;
using System.Collections.Generic;

public static class Util
{
    public static float DistanceToSegment(Vector2 point, Vector2 p1, Vector2 p2)
    {
        Vector2 closest;
        float dx = p2.x - p1.x;
        float dy = p2.y - p2.y;

        if (dx == 0 && dy == 0)
        {
            closest = p1;
            dx = point.x - p1.x;
            dy = point.y - p1.y;

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        float t = ((point.x - p1.x) * dx + (point.y - p1.y) * dy) /
            (dx * dx + dy * dy);

        if (t < 0)
        {
            closest = new Vector2(p1.x, p1.y);
            dx = point.x - p1.x;
            dy = point.y - p1.y;
        }
        else if (t > 1)
        {
            closest = new Vector2(p2.x, p2.y);
            dx = point.x - p2.x;
            dy = point.y - p2.y;
        }
        else
        {
            closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
            dx = point.x - closest.x;
            dy = point.y - closest.y;
        }

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static float DistanceToConnection(Connection c, Vector3 point, float maxDistance)
    {
        List<Vector2> connectionPoints = c.line.points;
        int pointsCount = connectionPoints.Count;
        float minDistance = Mathf.Infinity;

        for (int i = 1; i < pointsCount; i++)
        {
            float d = Util.DistanceToSegment(point, connectionPoints[i - 1], connectionPoints[i]);
            if (d < minDistance && d <= maxDistance)
            {
                minDistance = d;
            }
        }

        return minDistance;
    }

    public static Vector3 WorldToScreenPointInCanvas(Vector3 point, Manager m)
    {
        Camera cam = m.MainCam;
        RectTransform rect = m.canvasRectTransform;

        Vector3 screenPos = cam.WorldToScreenPoint(point);
        Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);
        Vector2 anchoredPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos2D, cam, out anchoredPos);
        return anchoredPos + new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2);
    }
}