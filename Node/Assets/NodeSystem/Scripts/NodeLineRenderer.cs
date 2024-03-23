using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeLineRenderer : Graphic
{
    RectTransform rectTransform;
    Canvas canvas;



}

public class NodeLine
{
    public enum ID { Start, End };

    public List<Vector2> Points;

    public float LineLength;

    public float GetDistance()
    {
        float d = 0;
        for (int i = 1; i < Points.Count; i++)
        {
            d += Vector2.Distance(Points[i - 1], Points[i]);
        }
        return d;
    }

    public void SetPoints(Vector2[] newPoints)
    {
        Points.Clear();
        Points.AddRange(newPoints);

        LineLength = GetDistance();
    }



}

public class NodeCurve
{
    public Vector2[] PivotPoints = new Vector2[4];
    public Vector2[] Points;
    public int Step;

    private int minPoints = 10;
    private int maxPoints = 20;

    public void SetPivotPoints(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        Step = CalculateStep(v1, v2, v3, v4);
        PivotPoints = new Vector2[] { v1, v2, v3, v4 };

        Points = GetCurvePoints(Step);
    }

    public int CalculateStep(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        float m = Mathf.Abs(Mathf.Sin(Vector2.Angle(v1 - v2, v4 - v2) * Mathf.Deg2Rad));
        float n = Mathf.Abs(Mathf.Sin(Vector2.Angle(v4 - v3, v1 - v3) * Mathf.Deg2Rad));
        float result = m > n ? m : n;

        return (int)((result * (maxPoints - minPoints)) + minPoints);
    }

    private Vector2[] GetCurvePoints(int s)
    {
        Points = new Vector2[s + 1];
        for (int i = 0; i < s; i++)
        {
            Points[i] = GetPoint(PivotPoints[0], PivotPoints[1], PivotPoints[2], PivotPoints[3], (float)(i / s));
        }

        Points[s] = PivotPoints[3];

        return Points;
    }

    public Vector3 GetPoint(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, float t)
    {
        float u = Mathf.Clamp01(t);
        float v = 1f - u;
        return (
            v1 * v * v * v +
            v2 * v * v * u * 3f +
            v3 * v * u * u * 3f +
            v4 * u * u * u
        );
    }

}